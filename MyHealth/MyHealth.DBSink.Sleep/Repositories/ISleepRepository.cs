using MyHealth.DBSink.Sleep.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Sleep.Repositories
{
    /// <summary>
    /// Interface to define contracts for the Sleep Repository
    /// </summary>
    public interface ISleepRepository
    {
        /// <summary>
        /// Adds a new sleep document to Cosmos DB
        /// </summary>
        /// <param name="sleep"></param>
        /// <returns></returns>
        Task AddSleep(SleepObject sleep);
    }
}
