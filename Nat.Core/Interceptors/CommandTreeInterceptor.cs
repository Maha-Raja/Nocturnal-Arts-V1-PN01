using System.Web;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;


namespace Nat.Core.Interceptors
{
    public class CommandTreeInterceptor : IDbCommandTreeInterceptor
    {
       
        const string createdByColumnName = "Created_By";
        const string createdDateColumnName = "Created_Date";
        const string lastUpdatedByColumnName = "Last_Updated_By";
        const string lastUpdatedDateColumnName = "Last_Updated_Date";

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            //if (ConfigurationSettings.AppSettings["LogSQL"] != null && ConfigurationSettings.AppSettings["LogSQL"] == "1")
            //{
            //    interceptionContext.DbContexts.First().Database.Log = s => { HttpContext.Current.Items["sql"] += s; };
            //}
            
            //string _tenantId, _userId;
            //getFromToken(out _tenantId, out _userId);

            if (interceptionContext.OriginalResult.DataSpace != DataSpace.SSpace)
            {
                return;
            }

            if (interceptionContext.Result as DbInsertCommandTree != null) //Insert query
            {
                var insertCommand = interceptionContext.Result as DbInsertCommandTree;

                // Add existing caluses except certain properties for extra saftey
                string tableName = insertCommand.Target.VariableType.EdmType.Name.ToString();
                List<DbModificationClause> finalSetClauses;
                finalSetClauses = new List<DbModificationClause>(
                    insertCommand.SetClauses.Cast<DbSetClause>()
                        //.Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != createdByColumnName)
                        .Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != createdDateColumnName)
                        //.Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != lastUpdatedByColumnName)
                        .Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != lastUpdatedDateColumnName));

                var properties = insertCommand.Target.VariableType.EdmType.MetadataProperties.Select(a => a.Value).ElementAt(1);
               
                // Create the variable reference in order to create the property
                var variableReference = DbExpressionBuilder.Variable(insertCommand.Target.VariableType,
                    insertCommand.Target.VariableName);

                ReadOnlyMetadataCollection<EdmMember> edmMembers = (ReadOnlyMetadataCollection<EdmMember>)properties;

                //if (edmMembers.Any(member => member.Name.Equals(createdByColumnName)))
                //{
                //    var property = DbExpressionBuilder.Property(variableReference, createdByColumnName);
                //    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromString(_userId));
                //    finalSetClauses.Add(setClause);
                //}

                if (edmMembers.Any(member => member.Name.Equals(createdDateColumnName)))
                {
                    var property = DbExpressionBuilder.Property(variableReference, createdDateColumnName);
                    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromDateTime(DateTime.UtcNow));
                    finalSetClauses.Add(setClause);
                }

                //if (edmMembers.Any(member => member.Name.Equals(lastUpdatedByColumnName)))
                //{
                //    var property = DbExpressionBuilder.Property(variableReference, lastUpdatedByColumnName);
                //    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromString(_userId));
                //    finalSetClauses.Add(setClause);
                //}

                if (edmMembers.Any(member => member.Name.Equals(lastUpdatedDateColumnName)))
                {
                    var property = DbExpressionBuilder.Property(variableReference, lastUpdatedDateColumnName);
                    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromDateTime(DateTime.UtcNow));
                    finalSetClauses.Add(setClause);
                }

                if (finalSetClauses != null)
                {
                    var newInsertCommand = new DbInsertCommandTree(
                        insertCommand.MetadataWorkspace,
                        insertCommand.DataSpace,
                        insertCommand.Target,
                        new ReadOnlyCollection<DbModificationClause>(finalSetClauses),
                        insertCommand.Returning);

                    interceptionContext.Result = newInsertCommand;
                }
            }
            else if (interceptionContext.Result as DbUpdateCommandTree != null) //Update query
            {
                var updateCommand = interceptionContext.Result as DbUpdateCommandTree;

                // Add existing caluses except certain properties for extra saftey
                string tableName = updateCommand.Target.VariableType.EdmType.Name.ToString();
                List<DbModificationClause> finalSetClauses;
               
                finalSetClauses = new List<DbModificationClause>(
                    updateCommand.SetClauses.Cast<DbSetClause>()
                        //.Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != lastUpdatedByColumnName)
                        .Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != lastUpdatedDateColumnName));

                var properties = updateCommand.Target.VariableType.EdmType.MetadataProperties.Select(a => a.Value).ElementAt(1);

                // Create the variable reference in order to create the property
                var variableReference = DbExpressionBuilder.Variable(updateCommand.Target.VariableType,
                    updateCommand.Target.VariableName);

                ReadOnlyMetadataCollection<EdmMember> edmMembers = (ReadOnlyMetadataCollection<EdmMember>)properties;

                //if (edmMembers.Any(member => member.Name.Equals(lastUpdatedByColumnName)))
                //{
                //    var property = DbExpressionBuilder.Property(variableReference, lastUpdatedByColumnName);
                //    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromString(_userId));
                //    finalSetClauses.Add(setClause);
                //}

                if (edmMembers.Any(member => member.Name.Equals(lastUpdatedDateColumnName)))
                {
                    var property = DbExpressionBuilder.Property(variableReference, lastUpdatedDateColumnName);
                    var setClause = DbExpressionBuilder.SetClause(property, DbExpression.FromDateTime(DateTime.UtcNow));
                    finalSetClauses.Add(setClause);
                }

                if (finalSetClauses != null)
                {
                    var newUpdateCommand = new DbUpdateCommandTree(
                        updateCommand.MetadataWorkspace,
                        updateCommand.DataSpace,
                        updateCommand.Target,
                        updateCommand.Predicate,
                        new ReadOnlyCollection<DbModificationClause>(finalSetClauses),
                        updateCommand.Returning);

                    interceptionContext.Result = newUpdateCommand;
                }
            }
        }





        private void getFromToken(out string tenantId, out string userId)
        {
            string tenantKey = "tenantId";            
            string userKey = "User";
            
            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            

            if (identity.FindFirst(tenantKey) != null && identity.FindFirst(userKey) != null)
            {
                tenantId = identity.FindFirst(tenantKey).Value;                
                userId = identity.FindFirst(userKey).Value;
            }
            else
            {                
                tenantId = "-1";
                userId = "not found in Thread.CurrentPrincipal.Identity";
            }
        }
    }
}
