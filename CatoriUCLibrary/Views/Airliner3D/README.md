# Regional 3D airliner

Copied from the independent AirplanelinerProp experiment. Studio already references this CatoriUCLibrary project. The control is `CatoriUCLibrary.Views.Airliner3D.Airplane3DUC`; its helper types use the same namespace to avoid collisions with existing controls. HelixToolkit.Wpf 3.1.2 is referenced by the library project.

```xml
xmlns:air="clr-namespace:CatoriUCLibrary.Views.Airliner3D;assembly=CatoriUCLibrary"
```

Place `<air:Airplane3DUC x:Name="RegionalAirliner"/>` in a shared 1200×680 scene with its background and authored 2D path; scale that scene together in a Viewbox. The transparent viewport covers the logical scene; use GetAircraftBounds() for the airplane's projected drop rectangle.

Included behavior: twin independent engines, 2D WPF PathGeometry following with automatic heading/bank, manual bank/pitch offsets, Side/Overhead/ThreeQuarter cameras and transitions, pilot/passenger boarding, automatic sequential engine startup and taxi, arrival/disembark events, trip reset/cancellation, and retractable landing gear.

For boarding: ResetTrip(), ResetPath(taxiPath), ConfigureTaxiPath(taxiPath), then BoardAsync(AirplaneOccupant) with pilot followed by passenger. DepartureTask tracks the remaining automatic sequence. Subscribe to OccupantBoarded/OccupantDisembarked to hide and restore host-owned avatars and carried items. The control retains identity and role, not PersonUC references. For ground taxi, set MaximumBankAngle to zero. ResetTrip restores the assigned route's start and lowers landing gear.

Use SetLandingGearAsync(false) to retract and SetLandingGearAsync(true) to lower. The gear must stay down during automatic boarding/taxi. Flight/engine/camera methods remain available independently for authored flight sequences. See the public partial files for events, cancellation and status properties.

This copy does not register a Studio palette entry, adapt Studio drag/drop interfaces, change serialization/code generation, replace the existing AirplaneUC, or modify generated game projects. Those are separate integration steps. It has no dependency on the standalone test application. Gear folds into the fuselage without modeled bay doors; the host supplies any visible boarded-avatar or door animations.
