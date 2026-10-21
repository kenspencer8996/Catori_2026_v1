# Avatar outfits and clothing packages

`AvatarOutfit`, `AvatarGarment`, and `AvatarClothingLayer` are UI-independent JSON data in CatoriInterfaces.AvatarOutfits. PersonUC.ApplyOutfit changes XAML clothing overlays on the existing registered visual parts; it does not rebuild the person or restart its animator.

## Using the rack

1. Place **Clothes Tree** from Studio Components and generate your game, or use ClothesTreeUC directly.
2. In the game, press **Open components folder** on the rack.
3. Copy each downloaded clothing package into its own subfolder, then press **Refresh**.
4. Press **New outfit**, edit its name, and drag components onto its hanger.
5. A component replaces the previous garment with the same Slot. Different slots combine; their layers can share a part.
6. Drag the hanger's picture onto a PersonUC. Outfits remain on the rack and are saved automatically.

The default folder is `%LOCALAPPDATA%/CatoriApp/Wardrobe`. Outfits are saved as an array of AvatarOutfit objects in `outfits.json`. Paths beneath this folder are saved relatively. Atomic saves keep `outfits.json.bak`. ClothesTreeUC.WardrobeFolder can select another folder.

## Downloadable package format

```
Components/
  YourPackage/
    garment.json
    thumbnail.png
    body.png
    left-upper-arm.png
    right-upper-arm.png
```

Example `garment.json` (this is documentation, not a built-in outfit):

```json
{
  "GarmentId": "your-stable-garment-id",
  "Name": "Your garment name",
  "Slot": "Top",
  "ThumbnailImage": "thumbnail.png",
  "Layers": [
    { "PartName": "Body", "ImagePath": "body.png", "LayerOrder": 10 },
    { "PartName": "LeftUpperArm", "ImagePath": "left-upper-arm.png", "LayerOrder": 10 },
    { "PartName": "RightUpperArm", "ImagePath": "right-upper-arm.png", "LayerOrder": 10 }
  ]
}
```

Each package must have a unique, stable GarmentId, a Name, Slot, thumbnail and at least one image layer. Files are local to the package; no executable code or network download logic is loaded. Invalid packages are reported on the rack without stopping valid packages from loading.

Common Slots are Top, Bottom, and Shoes; slot values are data and may be extended. Larger LayerOrder values draw above smaller values.

Supported existing part names: Body, LeftUpperArm, RightUpperArm, LeftForearm, RightForearm, LeftThigh, RightThigh, LeftLowerLeg, RightLowerLeg. Shoes use the lower-leg parts; they do not introduce new joints. A shoes package can add a higher layer over trouser layers on those same parts.

## Artwork alignment

Use transparent PNG artwork aligned with the target avatar's existing part drawing rectangles. Each clothing layer shares its base part's sizing, placement, stretch, visibility and transform. Transparent padding should place shoe artwork at the foot of the lower-leg rectangle. Changing a garment does not change pivots or geometry.

The base avatar's head and skin remain intact. Clothing already painted into a base sprite cannot be removed by clearing overlays; proper wardrobe artwork should target a matching segmented base avatar. Joint animation carries the layers automatically. Different baked full-body animation silhouettes need matching clothing artwork; this version does not generate pose-specific artwork.

## Drag payloads

Clothing component drags use `Catori.AvatarGarment` with AvatarGarment data. Outfit drags use `Catori.AvatarOutfit` with a snapshot of AvatarOutfit data. Neither payload transfers UI controls. Existing ticket, food and legacy IClothes drops remain supported.
