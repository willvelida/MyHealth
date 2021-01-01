using MyHealth.DBSink.Activity.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Repositories
{
    /// <summary>
    /// Interface to define contracts for Activity Repository
    /// </summary>
    public interface IActivityRepository
    {
        /// <summary>
        ///  Adds a new activity to Cosmos DB.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task AddActivity(ActivityObject activity);
    }
}
