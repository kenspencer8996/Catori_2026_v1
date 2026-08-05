using CatoriApp.Game.Objects.AnimationOnPath;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class Locationinterior3SubController : LocationSubControllerBase, ILocationSubController
    {
        public Locationinterior3SubController(FactoryInterior_UC view, long locationId)
            : base(locationId, view)
        {
            base._view = view;
            _locationId = locationId;
            //LoadLayoutItems();
            SetupAnimations();
        }
        // Generated code for location layout items
        ProductUC part15_productUC;
        PathAnimationHandle _pathHandlerConveyo11;
        ProductUC part16_productUC;
        PathAnimationHandle _pathHandlerConveyor21;
        ProductUC part17_productUC;
        PathAnimationHandle _pathHandlerConveyor31;
        ProductUC part19_productUC;
        PathAnimationHandle _pathHandlerConveyor5;
        ProductUC part21_productUC;
        PathAnimationHandle _pathHandlerConveyor13;

        private void SetupAnimations()
        {
            PathAnimationOptions? options = new PathAnimationOptions();
            options.Duration = TimeSpan.FromSeconds(4);
            options.InitialScale = .73;
            options.AutoStart = false;
            part15_productUC = new ProductUC("factories//SawHandle.png");
            // Code for Layout Item: Conveyo11
            string path15 = "M 292.09,416.37 L 266.94,420.12 L 254.83,422.5 L 243.65,425.88 L 233.42,430.26 L 224.12,435.64 L 219.93,438.49 L 216.19,441.16 L 212.91,443.64 L 210.07,445.94 L 207.69,448.06 L 205.76,449.99 L 204.28,451.74 L 204.02,452.13 Q 203.76,452.52 203.46,453.3 L 203.46,453.3 Q 203.17,454.09 203.4,454.94 L 203.4,454.94 Q 203.64,455.8 204.52,456.77 L 205.4,457.74 L 206.6,458.77 L 210.27,461.06 L 215.22,463.58 L 221.45,466.34 L 228.95,469.33 L 237.74,472.55 L 467.39,553.93 L 471.86,555.67 L 475.84,557.55 L 479.34,559.58 L 482.35,561.75 L 484.87,564.05 L 486.9,566.51 L 488.45,569.1 L 488.98,570.46 Q 489.51,571.83 489.79,573.27 L 490.08,574.71 L 490.17,577.73 L 489.77,580.89 L 488.89,584.19 L 487.51,587.64 L 485.65,591.22 L 483.3,594.95 L 480.47,598.82 L 476.2,604.28 L 469.85,611.91 L 463.12,619.04 L 456.01,625.66 L 448.51,631.79 L 440.63,637.41 L 432.37,642.54 L 423.72,647.16 L 414.69,651.28 L 278.18,708.58";
            _pathHandlerConveyo11 = GameAnimationHelper.AddControlOnPath("Conveyo11", _view.MainCanvas,
            part15_productUC, path15, options);

            part16_productUC = new ProductUC("");
            // Code for Layout Item: Conveyor21
            string path16 = "M 909.69,356.43 L 1034.93,370.35";
            _pathHandlerConveyor21 = GameAnimationHelper.AddControlOnPath("Conveyor21", _view.MainCanvas,
            part16_productUC, path16, options);

            part17_productUC = new ProductUC("factories//SawBlade.png");
            // Code for Layout Item: Conveyor31
            string path17 = "M 938.59,716.08 L 324.2,998.65";
            _pathHandlerConveyor31 = GameAnimationHelper.AddControlOnPath("Conveyor31", _view.MainCanvas,
            part17_productUC, path17, options);

            part19_productUC = new ProductUC("factories//SawHandle.png");
            // Code for Layout Item: Conveyor5
            string path19 = "M 1360.32,1069.3 L 1585.51,863.98 L 1599.56,849.96 L 1612.18,834.84 L 1623.34,818.62 L 1633.07,801.31 L 1660.21,747.51 L 1664.07,738.63 L 1666.65,729.84 L 1667.94,721.15 L 1667.95,712.55 L 1666.68,704.04 L 1664.13,695.63 L 1660.29,687.32 L 1655.17,679.1 L 1609.21,613.9 L 1606.53,609.84 L 1604.27,605.82 L 1602.42,601.84 L 1600.97,597.92 L 1599.94,594.03 L 1599.32,590.19 L 1599.11,586.4 L 1599.32,582.65 L 1599.93,578.95 L 1600.96,575.29 L 1602.39,571.67 L 1604.24,568.11 L 1606.5,564.58 L 1609.17,561.1 L 1612.25,557.67 L 1615.74,554.28 L 1741.37,439.92";
            _pathHandlerConveyor5 = GameAnimationHelper.AddControlOnPath("Conveyor5", _view.MainCanvas,
            part19_productUC, path19, options);

            part21_productUC = new ProductUC("factories//SawBlade.png");
            // Code for Layout Item: Conveyor13
            string path21 = "M 790.88,384.26 L 887.57,403.3 L 896.84,405.3 L 905.04,407.45 L 912.17,409.74 L 918.21,412.17 L 923.18,414.74 L 925.13,416.1 Q 927.08,417.46 928.49,418.89 L 928.49,418.89 Q 929.9,420.32 930.77,421.83 L 930.77,421.83 Q 931.64,423.33 931.94,424.95 L 931.94,424.95 Q 932.25,426.57 931.96,428.34 L 931.96,428.34 Q 931.67,430.12 930.79,432.05 L 930.79,432.05 Q 929.91,433.99 928.44,436.08 L 926.97,438.17 L 922.84,442.67 L 917.52,447.49 L 911.02,452.62 L 903.33,458.07 L 866.82,482.79 L 859.04,488.34 L 852.26,493.77 L 846.49,499.09 L 841.71,504.29 L 837.94,509.37 L 835.17,514.34 L 834.28,516.77 Q 833.4,519.2 833.01,521.57 L 833.01,521.57 Q 832.63,523.94 832.79,526.25 L 832.79,526.25 Q 832.96,528.57 833.72,530.82 L 833.72,530.82 Q 834.48,533.08 835.83,535.27 L 835.83,535.27 Q 837.19,537.47 839.14,539.61 L 841.1,541.74 L 846.19,545.9 L 852.48,549.95 L 859.96,553.87 L 868.63,557.68 L 884.39,564.06 L 903.1,570.82 L 922.17,576.08 L 941.58,579.86 L 961.35,582.14 L 965.07,582.42 L 985,583.46 L 1004.91,583.53 L 1024.8,582.61 L 1044.67,580.71 L 1054.08,579.57 L 1073.77,576.53 L 1093.13,572.21 L 1112.15,566.59 L 1130.83,559.68 L 1376.8,459.31 L 1381.24,457.43 L 1385.28,455.56 L 1388.94,453.7 L 1392.2,451.86 L 1395.08,450.04 L 1397.56,448.22 L 1399.66,446.42 L 1401.37,444.63 L 1402.68,442.86 L 1403.15,441.98 Q 1403.61,441.1 1403.88,440.22 L 1403.88,440.22 Q 1404.15,439.35 1404.22,438.48 L 1404.22,438.48 Q 1404.3,437.62 1404.18,436.76 L 1404.18,436.76 Q 1404.05,435.9 1403.74,435.05 L 1403.74,435.05 Q 1403.42,434.19 1402.91,433.35 L 1402.4,432.5 L 1400.99,430.82 L 1397.36,427.59 L 1392.9,424.58 L 1387.61,421.79 L 1381.48,419.22 L 1374.52,416.88 L 1366.73,414.76 L 1358.1,412.86 L 1348.64,411.19 L 1259.7,397.11";
            _pathHandlerConveyor13 = GameAnimationHelper.AddControlOnPath("Conveyor13", _view.MainCanvas,
            part21_productUC, path21, options);

        }

        public void StartProduction()
        {
            _pathHandlerConveyo11.Start();
            _pathHandlerConveyor21.Start();
            _pathHandlerConveyor31.Start();
            _pathHandlerConveyor5.Start();
            _pathHandlerConveyor13.Start();
        }
        public void StopProduction()
        {
            // Stop animations for all conveyors
            _pathHandlerConveyo11?.Stop();
            _pathHandlerConveyor21?.Stop();
            _pathHandlerConveyor31?.Stop();
             _pathHandlerConveyor5?.Stop();
             _pathHandlerConveyor13?.Stop();
         }

        public void LoadPaths()
        {
            // Paths are already loaded in SetupAnimations method
            // Add any additional path loading logic here if needed
        }

        public void OnAnimationCompleted()
        {
            // Handle animation completion logic
            // This can delegate to the base class method if needed
        }
    }
}
