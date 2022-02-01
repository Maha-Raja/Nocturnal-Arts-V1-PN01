using Nat.AuthApp.Services.ServiceModels;
using Nat.AuthApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.Exception;
using Nat.AuthApp.Models.Repositories;
using TLX.CloudCore.KendoX.UI;
using System.Data.Entity;
using TLX.CloudCore.KendoX.Extensions;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.AuthApp.Services.Services
{
    public class RoleService : BaseService<RoleModel, NAT_AUS_Role>
    {
        private static RoleService _service;
        public static RoleService GetInstance(NatLogger logger)
        {
            _service = new RoleService();
            _service.SetLogger(logger);
            return _service;
        }

        private RoleService() : base()
        {

        }
        public async Task<IEnumerable<RolePrivilegeMappingModel>> GetRolePrivileges(int roleId)
        {
            using (logger.BeginServiceScope("Get all active roles"))
            {
                try
                {
                    var privilegesDataModel = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetRolePrivilegesAsync(roleId);
                    if(privilegesDataModel.Count() != 0)
                    {
                        return new RolePrivilegeMappingModel().FromDataModelList(privilegesDataModel);
                    }
                    else
                    {
                        throw new Exception("Invalid Role!");
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        public async Task<RoleModel> GetRoleByIdAsync(int Id)
        {
            try
            {
                RoleModel data = null;
                NAT_AUS_Role roleModel = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleByIdAsync(Id);
                if (roleModel != null)
                {
                    data = new RoleModel().FromDataModel(roleModel);
                    //var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(roleModel.NAT_AUS_User_Role_Mapping);
                    //data.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                    return data;
                }
                throw new Exception("User with such Id not Found!");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<RoleModel> CreateRoleAsync(RoleModel obj)
        {
            try
            {
                
                IEnumerable<RolePrivilegeMappingModel> temproleprivilegemodel = new List<RolePrivilegeMappingModel>();
                IEnumerable<UserRoleMappingModel> userrolemapping = new List<UserRoleMappingModel>();

                obj.NatAusUserRoleMapping = new List<UserRoleMappingModel>();


                foreach (Int64 num in obj.MappedUserIds)
                {
                    UserRoleMappingModel tempmodel = new UserRoleMappingModel();
                    tempmodel.UserId = num;
                    obj.NatAusUserRoleMapping.Add(tempmodel);

                }



                temproleprivilegemodel = obj.NatAusRolePrivilegeMapping;
                userrolemapping = obj.NatAusUserRoleMapping;
                obj.NatAusRolePrivilegeMapping = null;
                obj.NatAusUserRoleMapping = null;

                obj.ActiveFlag = true;
                obj.ObjectState= ObjectState.Added;
                uow.Repository<NAT_AUS_Role>().Insert(obj.ToDataModel(obj));
                await uow.SaveChangesAsync();



                var roleId = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(obj.RoleCode);
                if (roleId != null)
                {
                    foreach (RolePrivilegeMappingModel priv in temproleprivilegemodel)
                    {
                        priv.RoleId = roleId.Value;
                        priv.ActiveFlag = true;
                        priv.ObjectState = ObjectState.Added;
                        uow.Repository<NAT_AUS_Role_Privilege_Mapping>().Insert(priv.ToDataModel(priv));

                    }

                    foreach (UserRoleMappingModel priv1 in userrolemapping)
                    {
                        priv1.RoleId = roleId.Value;
                        priv1.ActiveFlag = true;
                        priv1.ObjectState = ObjectState.Added;
                        uow.Repository<NAT_AUS_User_Role_Mapping>().Insert(priv1.ToDataModel(priv1));

                    }
                    await uow.SaveChangesAsync();
                    return obj;

                }
                else
                {
                    throw new Exception("error");
                }

            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }



        }

        public async Task<RoleModel> UpdateRole(RoleModel obj)
        {
            IEnumerable<RolePrivilegeMappingModel> newroleprivilegemodel = new List<RolePrivilegeMappingModel>();
            IEnumerable<RolePrivilegeMappingModel> roleprivilegemodel = new List<RolePrivilegeMappingModel>();
            IEnumerable<UserRoleMappingModel> newuserrolemodel = new List<UserRoleMappingModel>();
            IEnumerable<UserRoleMappingModel> userrolemodel = new List<UserRoleMappingModel>();

            obj.NatAusUserRoleMapping = new List<UserRoleMappingModel>();


            foreach (Int64 num in obj.MappedUserIds)
            {
                UserRoleMappingModel tempmodel = new UserRoleMappingModel();
                tempmodel.UserId = num;
                tempmodel.RoleId = obj.RoleId;
                obj.NatAusUserRoleMapping.Add(tempmodel);

            }
            

            newroleprivilegemodel = obj.NatAusRolePrivilegeMapping;
            newuserrolemodel = obj.NatAusUserRoleMapping;
            obj.NatAusRolePrivilegeMapping = null;
            obj.NatAusUserRoleMapping = null;
            obj.LastUpdatedDate = System.DateTime.UtcNow;
            obj.ObjectState = ObjectState.Modified;
            uow.Repository<NAT_AUS_Role>().Update(obj.ToDataModel(obj));
            var roleid = obj.RoleId;

            IEnumerable<NAT_AUS_Role_Privilege_Mapping> rolepriv = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetRolePrivilegesMappingByRoleID(Convert.ToInt32(roleid));

            IEnumerable<RolePrivilegeMappingModel> oldroleprivmodel = new RolePrivilegeMappingModel().FromDataModelList(rolepriv);

            IEnumerable<NAT_AUS_User_Role_Mapping> userrolepriv = await uow.RepositoryAsync<NAT_AUS_User_Role_Mapping>().GetUserRoleMappingByRoleID(Convert.ToInt32(roleid));

            IEnumerable<UserRoleMappingModel> olduserroleprivmodel = new UserRoleMappingModel().FromDataModelList(userrolepriv);

            var olduserIds = olduserroleprivmodel.Select(x => x.UserId).ToArray();
            var newuserIds = newuserrolemodel.Select(x => x.UserId).ToArray();

            IEnumerable<UserRoleMappingModel> olduserroleprivmodel12 = new List<UserRoleMappingModel>();
            olduserroleprivmodel12 = olduserroleprivmodel.Where(t => newuserIds.Contains(t.UserId));
            userrolemodel = newuserrolemodel.Where(t => olduserIds.Contains(t.UserId));

            newuserrolemodel = newuserrolemodel.Except(userrolemodel).ToList();
            olduserroleprivmodel = olduserroleprivmodel.Except(olduserroleprivmodel12).ToList();

            foreach (UserRoleMappingModel aa in olduserroleprivmodel)
            {
                if (!newuserIds.Contains(aa.UserId))
                {
                    aa.ObjectState = ObjectState.Deleted;
                    uow.RepositoryAsync<NAT_AUS_User_Role_Mapping>().Delete(aa.ToDataModel(aa));

                }
            }
            if (newuserrolemodel != null)
            {
                foreach (UserRoleMappingModel priv in newuserrolemodel)
                {
                    priv.ActiveFlag = true;
                    priv.ObjectState = ObjectState.Added;
                    uow.Repository<NAT_AUS_User_Role_Mapping>().Insert(priv.ToDataModel(priv));

                }
            }



            var oldIds = oldroleprivmodel.Select(x => x.PrivilegeId).ToArray();
            var newIds = newroleprivilegemodel.Select(x => x.PrivilegeId).ToArray();

            IEnumerable<RolePrivilegeMappingModel> oldroleprivmodel12 = new List<RolePrivilegeMappingModel>();
            oldroleprivmodel12 = oldroleprivmodel.Where(t => newIds.Contains(t.PrivilegeId));
            roleprivilegemodel = newroleprivilegemodel.Where(t => oldIds.Contains(t.PrivilegeId));


            newroleprivilegemodel = newroleprivilegemodel.Except(roleprivilegemodel).ToList();
            oldroleprivmodel = oldroleprivmodel.Except(oldroleprivmodel12).ToList();


            foreach(RolePrivilegeMappingModel aa in oldroleprivmodel)
            {
                if (!newIds.Contains(aa.PrivilegeId))
                {
                    aa.ObjectState = ObjectState.Deleted;
                    uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().Delete(aa.ToDataModel(aa));

                }
            }
            if(newroleprivilegemodel !=null)
            {
                foreach (RolePrivilegeMappingModel priv in newroleprivilegemodel)
                {
                    priv.ActiveFlag = true;
                    priv.ObjectState = ObjectState.Added;
                    uow.Repository<NAT_AUS_Role_Privilege_Mapping>().Insert(priv.ToDataModel(priv));

                }
            }



            int updatedRows = await uow.SaveChangesAsync();

            if (updatedRows == 0)
            {
                return obj;
            }
            return obj;
        }

        public async Task<IEnumerable<PrivilegeModel>> GetAllPrivileges()
        {
            using (logger.BeginServiceScope("Get all active roles"))
            {
                try
                {
                    var privilegesDataModel = await uow.RepositoryAsync<NAT_AUS_Privilege>().GetallPrivilegesAsync();
                    if (privilegesDataModel.Count() != 0)
                    {
                        return new PrivilegeModel().FromDataModelList(privilegesDataModel);
                    }
                    else
                    {
                        throw new Exception("no privileges");
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<IEnumerable<RoleModel>> GetAllActiveRoles()
        {
            using (logger.BeginServiceScope("Get all active roles"))
            {
                try
                {
                    var rolesDataModel = await uow.RepositoryAsync<NAT_AUS_Role>().GetAllActiveRolesAsync();
                    return new RoleModel().FromDataModelList(rolesDataModel);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        public async Task<DataSourceResult> GetAllRoleListAsync(DataSourceRequest request)
        {
            try
            {
                var roledata = await uow.RepositoryAsync<NAT_AUS_Role>().Queryable()

                              .Include(x => x.NAT_AUS_User_Role_Mapping)
                              .Include(x => x.NAT_AUS_Role_Privilege_Mapping)
                              .ToListAsync();

                var rolelistModel = new RoleModel().FromDataModelList(roledata);
                var result = rolelistModel.ToDataSourceResult(request);



                return result;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}
