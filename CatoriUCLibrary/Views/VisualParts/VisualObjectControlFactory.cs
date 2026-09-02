using System.IO;
using System.Text.Json;
using System.Windows;
using CatoriUCLibrary.Views.Airplane;
using CatoriUCLibrary.Views.Person;
using CatoriUCLibrary.Views.Tickets;

namespace CatoriUCLibrary.Views.VisualParts;

public static class VisualObjectControlFactory
{
    public static FrameworkElement? Create(string? kind,string? definitionPath)
    {
        if (string.Equals(kind, "TicketKiosk", StringComparison.OrdinalIgnoreCase))
        {
            return new TicketKioskUC();
        }
        if(string.IsNullOrWhiteSpace(definitionPath)||!File.Exists(definitionPath))return null;
        if(string.Equals(kind,"Avatar",StringComparison.OrdinalIgnoreCase))return CreateAvatar(definitionPath);
        if(!string.Equals(kind,"Airplane",StringComparison.OrdinalIgnoreCase))return null;
        using JsonDocument document=JsonDocument.Parse(File.ReadAllText(definitionPath));
        if(!document.RootElement.TryGetProperty("Parts",out JsonElement parts))return null;
        VisualPartConfiguration? Part(string name)
        {
            if(!parts.TryGetProperty(name,out JsonElement value)
                ||!value.TryGetProperty("IsComplete",out var complete)||!complete.GetBoolean())return null;
            string image=value.TryGetProperty("CleanedImagePath",out var cleaned)?cleaned.GetString()??string.Empty:string.Empty;
            if(string.IsNullOrWhiteSpace(image)||!File.Exists(image))return null;
            double x=Number(value,"OriginalX"),y=Number(value,"OriginalY"),w=Number(value,"Width"),h=Number(value,"Height");
            double px=Number(value,"PivotSourceX"),py=Number(value,"PivotSourceY");
            return new VisualPartConfiguration{Name=name,ImagePath=image,X=x,Y=y,Width=w,Height=h,
                PivotX=w<=0?.5:(px-x)/w,PivotY=h<=0?.5:(py-y)/h,
                InitialAngle=Number(value,"InitialAngle"),ZOrder=(int)Number(value,"ZOrder")};
        }
        var body=Part("Body")??new VisualPartConfiguration{Name="Body",Width=1,Height=1};
        var gear=new List<VisualPartConfiguration>();
        foreach(JsonProperty property in parts.EnumerateObject())
            if(property.Name.StartsWith("LandingGear",StringComparison.OrdinalIgnoreCase)
                &&!string.Equals(property.Name,"LandingGear",StringComparison.OrdinalIgnoreCase)
                &&Part(property.Name) is { } item)gear.Add(item);
        var control=new AirplaneUC();control.ApplySettings(new AirplaneSettings{Body=body,LeftWing=Part("LeftWing"),RightWing=Part("RightWing"),
            LeftPropeller=Part("LeftPropeller"),RightPropeller=Part("RightPropeller"),Door=Part("Door"),LandingGear=Part("LandingGear"),LandingGearParts=gear});
        return control;
    }

    private static FrameworkElement? CreateAvatar(string definitionPath)
    {
        using JsonDocument project=JsonDocument.Parse(File.ReadAllText(definitionPath));
        string exportFolder=project.RootElement.TryGetProperty("ExportFolder",out JsonElement folderValue)
            ?folderValue.GetString()??string.Empty:string.Empty;
        if(string.IsNullOrWhiteSpace(exportFolder))return null;
        if(!Path.IsPathRooted(exportFolder))
            exportFolder=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(definitionPath)??string.Empty,exportFolder));
        string metadataPath=Path.Combine(exportFolder,"avatar.json");
        if(!File.Exists(metadataPath))return null;
        using JsonDocument metadata=JsonDocument.Parse(File.ReadAllText(metadataPath));
        if(!metadata.RootElement.TryGetProperty("Parts",out JsonElement parts)
            ||parts.ValueKind!=JsonValueKind.Array)return null;

        var values=new Dictionary<string,JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach(JsonElement part in parts.EnumerateArray())
        {
            string name=part.TryGetProperty("PartType",out JsonElement type)
                ?type.ValueKind==JsonValueKind.String?type.GetString()??string.Empty:type.GetRawText()
                :string.Empty;
            if(!string.IsNullOrWhiteSpace(name))values[name]=part.Clone();
        }
        if(values.Count==0)return null;
        double minX=values.Values.Min(value=>Number(value,"OriginalX"));
        double minY=values.Values.Min(value=>Number(value,"OriginalY"));
        double maxX=values.Values.Max(value=>Number(value,"OriginalX")+Number(value,"Width"));
        double maxY=values.Values.Max(value=>Number(value,"OriginalY")+Number(value,"Height"));
        double sourceWidth=Math.Max(1,maxX-minX),sourceHeight=Math.Max(1,maxY-minY);
        const double designWidth=300,designHeight=400,margin=10;
        double scale=Math.Min((designWidth-margin*2)/sourceWidth,(designHeight-margin*2)/sourceHeight);
        double originX=(designWidth-sourceWidth*scale)/2,originY=(designHeight-sourceHeight*scale)/2;

        AvatarPartSettings Part(string name)
        {
            if(!values.TryGetValue(name,out JsonElement value))return new AvatarPartSettings();
            string filename=value.TryGetProperty("PngFilename",out JsonElement file)?file.GetString()??string.Empty:string.Empty;
            string image=string.IsNullOrWhiteSpace(filename)?string.Empty:Path.Combine(exportFolder,filename);
            double width=Number(value,"Width"),height=Number(value,"Height");
            return new AvatarPartSettings
            {
                ImagePath=File.Exists(image)?image:string.Empty,
                X=originX+(Number(value,"OriginalX")-minX)*scale,
                Y=originY+(Number(value,"OriginalY")-minY)*scale,
                Width=Math.Max(1,width*scale),Height=Math.Max(1,height*scale),
                PivotX=width<=0?.5:Number(value,"PivotX")/width,
                PivotY=height<=0?.5:Number(value,"PivotY")/height,
                Offset=Number(value,"Offset")*scale,
                InitialAngle=Number(value,"InitialAngle"),
                ZIndex=(int)Number(value,"ZOrder")
            };
        }
        AvatarPartSettings body=Part("Body"),head=Part("Head"),leftArm=Part("LeftUpperArm"),leftForearm=Part("LeftLowerArm"),
            rightArm=Part("RightUpperArm"),rightForearm=Part("RightLowerArm"),leftLeg=Part("LeftUpperLeg"),
            leftLowerLeg=Part("LeftLowerLeg"),rightLeg=Part("RightUpperLeg"),rightLowerLeg=Part("RightLowerLeg");
        MakeRelative(leftForearm,leftArm);MakeRelative(rightForearm,rightArm);
        MakeRelative(leftLowerLeg,leftLeg);MakeRelative(rightLowerLeg,rightLeg);
        var settings=PersonAvatarSettings.CreateDefault();
        settings.Name=metadata.RootElement.TryGetProperty("Name",out JsonElement nameValue)?nameValue.GetString()??"Avatar":"Avatar";
        settings.DesignWidth=designWidth;settings.DesignHeight=designHeight;
        settings.Body=body;settings.Head=head;settings.LeftArm=leftArm;settings.LeftForearm=leftForearm;
        settings.RightArm=rightArm;settings.RightForearm=rightForearm;settings.LeftLeg=leftLeg;
        settings.LeftLowerLeg=leftLowerLeg;settings.RightLeg=rightLeg;settings.RightLowerLeg=rightLowerLeg;
        var control=new PersonUC{IsDragEnabled=false,IsPartEditingEnabled=false};
        control.ApplySettings(settings);
        return control;
    }

    private static void MakeRelative(AvatarPartSettings child,AvatarPartSettings parent)
    {child.X-=parent.X;child.Y-=parent.Y;}
    private static double Number(JsonElement value,string name)=>value.TryGetProperty(name,out var property)&&property.TryGetDouble(out double number)?number:0;
}
