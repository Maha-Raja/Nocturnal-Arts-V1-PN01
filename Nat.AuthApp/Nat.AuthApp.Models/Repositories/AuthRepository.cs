using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.AuthApp.Models.EFModel;
using System.Threading.Tasks;
using System;

namespace Nat.AuthApp.Models.Repositories
{
    public static class AuthRepository
    {
        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <param name="repository">User Repository</param>
        /// <returns>Collection of User EF model</returns>
        public static async Task<IEnumerable<NAT_AUS_User>> GetAllActiveUsersAsync(this IRepositoryAsync<NAT_AUS_User> repository)
        {
            return await repository.Queryable().Where(x=>x.Active_Flag == true).Include(x => x.NAT_AUS_User_Location_Mapping)
                .Include(x => x.NAT_AUS_User_Role_Mapping.Select(y => y.NAT_AUS_Role)).ToListAsync();
        }

        /// <summary>
        /// This method returns User Model by Ref Type and Ref Id
        /// </summary>
        /// <param name="repository">User Repository</param>
        /// <param name="refType">Ref Type of User</param>
        /// <param name="refId">Ref Id of User</param>
        /// <returns>User EF model</returns>
        public static async Task<NAT_AUS_User> GetActiveUserByRef(this IRepositoryAsync<NAT_AUS_User> repository, string refType, long refId)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Reference_ID == refId && x.Reference_Type_LKP == refType).FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> GetUserByRef(this IRepositoryAsync<NAT_AUS_User> repository, string refType, long refId)
        {
            return await repository.Queryable().Where(x => x.Reference_ID == refId && x.Reference_Type_LKP == refType).FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> GetInActiveUserByRef(this IRepositoryAsync<NAT_AUS_User> repository, string refType, long refId)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == false && x.Reference_ID == refId && x.Reference_Type_LKP == refType).FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> GetActiveUserByRefID(this IRepositoryAsync<NAT_AUS_User> repository, long refId)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Reference_ID == refId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns list of all users with specific reference type passed as parameter
        /// </summary>
        /// <param name="repository">User Repository</param>
        /// <param name="refType">Ref Type of User</param>
        /// <returns>Collection of User EF model</returns>
        public static async Task<IEnumerable<NAT_AUS_User>> GetAllUsersByReferenceType(this IRepositoryAsync<NAT_AUS_User> repository, string refType)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Reference_Type_LKP == refType).ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_User>> GetAllUsers(this IRepositoryAsync<NAT_AUS_User> repository)
        {

            return await repository.Queryable().Where(x => x.Active_Flag == true)
                .Include(x => x.NAT_AUS_User_Location_Mapping)
                .Include(x => x.NAT_AUS_User_Role_Mapping.Select(y => y.NAT_AUS_Role)).ToListAsync();
        }

        /// <summary>
        /// Retrieve salt of a user if found else returns null
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<string> GetSaltByUsernameAsync(this IRepositoryAsync<NAT_AUS_User> repository, string username)
        {
            return await repository.Queryable().Where(x => x.User_Name == username && x.Active_Flag == true).Select(x => x.Password_Salt).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieve user by id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<NAT_AUS_User> GetUserByIdAsync(this IRepositoryAsync<NAT_AUS_User> repository, long id)
        {
            return await repository.Queryable().Where(x => x.User_ID == id)
                .Include(x => x.NAT_AUS_User_Location_Mapping)
                .Include(x => x.NAT_AUS_User_Role_Mapping.Select(y => y.NAT_AUS_Role.NAT_AUS_Role_Privilege_Mapping.Select(l => l.NAT_AUS_Privilege)))
                .FirstOrDefaultAsync();
        }
        public static async Task<NAT_AUS_User> GetArtistUserByArtistIdAsync(this IRepositoryAsync<NAT_AUS_User> repository, long id)
        {
            return await repository.Queryable().Where(x => x.Reference_ID == id && x.Reference_Type_LKP == "artist")
                .Include(x => x.NAT_AUS_User_Location_Mapping)
                .Include(x => x.NAT_AUS_User_Role_Mapping.Select(y => y.NAT_AUS_Role.NAT_AUS_Role_Privilege_Mapping.Select(l => l.NAT_AUS_Privilege)))
                .FirstOrDefaultAsync();
        }
        public static async Task<NAT_AUS_Role> GetRoleByIdAsync(this IRepositoryAsync<NAT_AUS_Role> repository, long id)
        {
            return await repository.Queryable().Where(x => x.Role_ID == id)
                .Include(x => x.NAT_AUS_User_Role_Mapping)
                .Include(x => x.NAT_AUS_Role_Privilege_Mapping)
                .FirstOrDefaultAsync();
        }
        public static async Task<IEnumerable<NAT_AUS_Notification_Preference>> GetNotificationPreferencesByUserIdAsync(this IRepositoryAsync<NAT_AUS_Notification_Preference> repository, long id)
        {
            return await repository.Queryable().Where(x => x.User_ID == id && x.Active_Flag == true).ToListAsync();
        }

        /// <summary>
        /// Retrieve salt of a user if found else returns null
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NAT_AUS_Role_Privilege_Mapping>> GetUserPrivilegesAsync(this IRepositoryAsync<NAT_AUS_Role_Privilege_Mapping> repository, IEnumerable<NAT_AUS_User_Role_Mapping> user_repo)
        {
            //  return await repository.Queryable().Include(x => x.NAT_AUS_Privilege).Where (x => user_repo.Select(y => y.Role_ID).Contains(x.Role_ID)).ToListAsync();
            Func<NAT_AUS_Role_Privilege_Mapping, bool> whereCondition = (i) =>
            {
                return  user_repo.Select(y => y.Role_ID).Contains(i.Role_ID);
            };

            return repository.Queryable()
                .Include(x => x.NAT_AUS_Privilege)
                .Where(whereCondition).ToList();

        }


        public static async Task<NAT_AUS_External_Identity> GetExternalIdentityUserAsync(this IRepositoryAsync<NAT_AUS_External_Identity> repository, string username, string identityprovider)
        {
            return await repository.Queryable().Where(x => x.Account_ID == username && x.Active_Flag == true && x.Identity_Provider_LKP == identityprovider).FirstOrDefaultAsync();
        }


        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <param name="repository">User Repository</param>
        /// <returns>Collection of User EF model</returns>
        public static async Task<NAT_AUS_User> GetUserForLoginAsync(this IRepositoryAsync<NAT_AUS_User> repository, string username, string passwordHash)
        {
            return await repository.Queryable().Where(x => x.User_Name == username && x.Password_Hash == passwordHash).FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> getDuplicateEmail(this IRepositoryAsync<NAT_AUS_User> repository, string email)
        {
            return await repository.Queryable().Where(x => x.User_Name == email || x.Email == email).FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> getDuplicatePhoneNumber(this IRepositoryAsync<NAT_AUS_User> repository, string phoneNumber)
        {
            return await repository.Queryable().Where(x => x.User_Name == phoneNumber || x.Phone_Number == phoneNumber).FirstOrDefaultAsync();
        }


        /// <summary>
        /// This method search user by username
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<NAT_AUS_User> GetUserByUsernameAsync(this IRepositoryAsync<NAT_AUS_User> repository, string username)
        {
            return await repository.Queryable().Where(x => x.Email == username || x.Phone_Number == username)
                                               .Include(x => x.NAT_AUS_User_Role_Mapping
                                               .Select(y => y.NAT_AUS_Role))
                                               .FirstOrDefaultAsync();
        }

        public static async Task<NAT_AUS_User> GetUserByUsername(this IRepositoryAsync<NAT_AUS_User> repository, string username)
        {
            return await repository.Queryable().Where(x => x.Email == username || x.Phone_Number == username).AsNoTracking().Include(x => x.NAT_AUS_User_Role_Mapping).AsNoTracking().FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns list of all active roles
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Role EF model</returns>
        public static async Task<IEnumerable<NAT_AUS_Role>> GetAllActiveRolesAsync(this IRepositoryAsync<NAT_AUS_Role> repository)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true).ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_Role_Privilege_Mapping>> GetRolePrivilegesAsync(this IRepositoryAsync<NAT_AUS_Role_Privilege_Mapping> repository,int userRole)
        {
            return await repository.Queryable().Where(x => x.Role_ID == userRole).Include(x => x.NAT_AUS_Privilege).ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_User_Role_Mapping>> GetRoleusersAsync(this IRepositoryAsync<NAT_AUS_User_Role_Mapping> repository, int userRole)
        {
            return await repository.Queryable().Where(x => x.Role_ID == userRole).Include(x => x.NAT_AUS_User).Include("NAT_AUS_User.NAT_AUS_User_Location_Mapping").ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_User>> GetusersbyManagerIdAsync(this IRepositoryAsync<NAT_AUS_User> repository, int ManagerID)
        {
            return await repository.Queryable().Where(x => x.Reporting_Manager == ManagerID).ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_Role_Privilege_Mapping>> GetRolePrivilegesMappingByRoleID(this IRepositoryAsync<NAT_AUS_Role_Privilege_Mapping> repository, int roleID)
        {
            return await repository.Queryable().Where(x => x.Role_ID == roleID).AsNoTracking().ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_User_Role_Mapping>> GetUserRoleMappingByRoleID(this IRepositoryAsync<NAT_AUS_User_Role_Mapping> repository, int roleID)
        {
            return await repository.Queryable().Where(x => x.Role_ID == roleID).AsNoTracking().ToListAsync();
        }

        public static async Task<IEnumerable<NAT_AUS_Privilege>> GetallPrivilegesAsync(this IRepositoryAsync<NAT_AUS_Privilege> repository)
        {
            return await repository.Queryable().ToListAsync();
        }

        /// <summary>
        /// Returns RoleID against the given code if Role does not exists returns null
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public static async Task<long?> GetRoleIDByRoleCode(this IRepositoryAsync<NAT_AUS_Role> repository, string roleCode)
        {
            return await repository.Queryable().Where(x => x.Role_Code == roleCode).Select(x => (long?)x.Role_ID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns list of all active roles
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NAT_AUS_Role>> GetRoleIDByRoleCode(this IRepositoryAsync<NAT_AUS_Role> repository)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true).ToListAsync();
        }
    }
}
