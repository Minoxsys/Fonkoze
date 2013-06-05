using Core.Domain;
using Core.Persistence;
using System;

namespace Web.Models.RoleManager
{
    public class RoleManagerAssignModel
    {
        private readonly IQueryService<Role> _queryRole;
        private readonly IQueryService<Permission> _queryFunction;
        private readonly ISaveOrUpdateCommand<Role> _updateRole;

        public RoleManagerAssignModel(IQueryService<Role> queryRole,
                                      IQueryService<Permission> queryFunction,
                                      ISaveOrUpdateCommand<Role> updateRole)
        {
            _queryFunction = queryFunction;
            _queryRole = queryRole;
            _updateRole = updateRole;
        }

        public virtual void LinkFunctionToRole(Guid functionId, Guid roleId)
        {
            var function = _queryFunction.Load(functionId);
            var role = _queryRole.Load(roleId);

            role.AddFunction(function);

            _updateRole.Execute(role);
        }
    }
}