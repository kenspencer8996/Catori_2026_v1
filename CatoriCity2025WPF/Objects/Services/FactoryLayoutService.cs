using System.Collections.Generic;

public class FactoryLayoutService
{
    private readonly FactoryLayoutRepository _repo;
    private readonly FactoryLayoutPointRepository _pointRepo;
    private readonly FactoryAssemblyRobotsRepository _robotRepo;
    public FactoryLayoutService()
    {
        _repo = new FactoryLayoutRepository();
        _pointRepo = new FactoryLayoutPointRepository();
        _robotRepo = new FactoryAssemblyRobotsRepository();
    }

    public void Create(FactoryLayoutEntity layout) => _repo.Insert(layout);

    public void Update(FactoryLayoutEntity layout) => _repo.Update(layout);

    public void Delete(long id) => _repo.Delete(id);

    public FactoryLayoutViewModel? Get(long id)
    {
        FactoryLayoutEntity result = _repo.GetById(id);
        FactoryLayoutViewModel viewModel = new FactoryLayoutViewModel();
        if (result != null)
        {
            result.Points = _pointRepo.GetListByLayoutId(id);
        }
        viewModel.ToViewModel(result);
        return viewModel;
    }

    public FactoryLayoutViewModel? GetByFactoryInteriorName(string name)
    {
        FactoryLayoutEntity result = _repo.GetByFactoryInteriorName(name);
        FactoryLayoutViewModel viewModel = new FactoryLayoutViewModel();
        if (result != null)
        {
            result.Points = _pointRepo.GetListByLayoutId(result.FactoryLayoutId);
            viewModel.ToViewModel(result);
        }
        return viewModel;
    }
    public void UpdatePoints(long layoutId, FactoryLayoutPointEntity point)
    {
        List<FactoryLayoutPointEntity> found = _pointRepo.GetByLayoutId(layoutId);
        // find and update the point if it exists, otherwise insert it
        bool pointExists = false;
        if (found != null && found.Count > 0)
        { 
            var existingPoint = found.Find(p => p.PointType == point.PointType);
            if (existingPoint != null)
            {
                pointExists = true;
            }
        }
        if (pointExists)
        {
            _pointRepo.Update( point);
        }
        else
            _pointRepo.Insert( point);
    }

    // Robot operations
    public List<FactoryAssemblyRobotEntity> GetAllRobots()
    {
        return _robotRepo.GetAll();
    }

    public FactoryAssemblyRobotEntity? GetRobot(long id)
    {
        return _robotRepo.GetById(id);
    }

    public long AddRobot(FactoryAssemblyRobotEntity robot)
    {
        return _robotRepo.Insert(robot);
    }

    public void UpdateRobot(FactoryAssemblyRobotEntity robot)
    {
        _robotRepo.Update(robot);
    }

    public void DeleteRobot(long id)
    {
        _robotRepo.Delete(id);
    }

    // ---------------------------------------------------------
    // Convenience / High-level operations
    // ---------------------------------------------------------

   
    public List<FactoryAssemblyRobotEntity> GetRobotsForFactory(long factoryLayoutId)
    {
        return _robotRepo.GetbyFactoryLayoutId(factoryLayoutId);
                   
    }
    public void UpdateRobotPosition(long robotId, long x, long y)
    {
        var robot = _robotRepo.GetById(robotId);
        if (robot == null)
            return;

        robot.Xloc = x;
        robot.Yloc = y;

        _robotRepo.Update(robot);
    }

    public void UpdateRobotSize(long robotId, long width, long height)
    {
        var robot = _robotRepo.GetById(robotId);
        if (robot == null)
            return;

        robot.Width = width;
        robot.Height = height;

        _robotRepo.Update(robot);
    }

    public FactoryAssemblyRobotEntity CreateRobot(string name, long factoryLayoutId, string robotType, double x, double y, double width, double height)
    {
        var robot = new FactoryAssemblyRobotEntity
        {
            Name = name,
            FactoryLayoutId = factoryLayoutId,
            RobotType = robotType,
            Xloc = x,
            Yloc = y,
            Width = width,
            Height = height
        };
        long newId = _robotRepo.Insert(robot);
        var result = _robotRepo.GetById(newId);
        return result;
    }

    // ---------------------------------------------------------
    // Drag-and-drop integration
    // ---------------------------------------------------------

    public void SaveRobotDragPosition(FactoryAssemblyRobotEntity robot, double x, double y)
    {
        robot.Xloc = (long)x;
        robot.Yloc = (long)y;
        _robotRepo.Update(robot);
    }
}