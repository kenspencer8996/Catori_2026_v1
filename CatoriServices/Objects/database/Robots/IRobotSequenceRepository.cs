namespace CatoriServices.Objects.database.Robots
{
    public interface IRobotSequenceRepository
    {
        Task<List<RobotSequenceEntity>> GetAllAsync();
        Task<RobotSequenceEntity?> GetByIdAsync(long robotSequenceId);
        Task<List<RobotSequenceEntity>> GetByLocationIdAsync(long locationId);
        Task<bool> UpdateAsync(RobotSequenceEntity sequence);
    }
}
