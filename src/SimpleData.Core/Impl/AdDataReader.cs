using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;

namespace SimpleData.Core
{
    public class AdDataReader : IDataReader
    {

        public AdDataReader(string ldap, string username, string password, QuerySchema querySchema) : this(new string[] { ldap }, username, password, querySchema)
        {
        }

        public AdDataReader(string[] ldaps, string username, string password, QuerySchema querySchema)
        {
            Ldaps = ldaps;
            UserName = username;
            Password = password;
            UserFilter = "(&(objectClass=user)(objectCategory=person))";
            OrganizationalUnitFilter = "(&(objectClass=organizationalUnit))";
            GroupFilter = "(&(objectClass=group))";
            QuerySchema = querySchema;
        }

        public QuerySchema QuerySchema { get; set; }
        public string[] Ldaps { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string UserFilter { get; set; }
        public string OrganizationalUnitFilter { get; set; }
        public string GroupFilter { get; set; }
        public string UserIdField { get; set; }
        public string OrganizationIdField { get; set; }
        public string GroupIdField { get; set; }
        public string UserParentIdField { get; set; }
        public string OrganizationParentIdField { get; set; }
        public string GroupParentIdField { get; set; }


        public List<string> OrganizationalUnitProperties = new List<string>{
                "name",
                "whenChanged",
                "whenCreated"
            };

        public List<string> UserProperties = new List<string>{
                "name",
                "displayName",
                "sAMAccountName",
                "userPrincipalName",
                "mail",
                "description",
                "company",
                "department",
                "physicalDeliveryOfficeName",
                "employeeNumber",
                "mobile"
            };

        public List<string> GroupProperties = new List<string>{
                "name",
                "whenChanged",
                "whenCreated"
            };

        public IEnumerable<DataSet> Get()
        {
            var result = new List<DataSet>();
            if((QuerySchema & QuerySchema.group) != 0)
            {
                result.Add(new DataSet()
                {
                    Data = GetGroups(),
                    Name = "Group"
                });
            }
            if ((QuerySchema & QuerySchema.user) != 0)
            {
                result.Add(new DataSet()
                {
                    Data = GetUsers(),
                    Name = "User"
                });
            }
            if ((QuerySchema & QuerySchema.organizationalUnit) != 0)
            {
                result.Add(new DataSet()
                {
                    Data = GetDepartments(),
                    Name = "Organization"
                });
            }
            return result;
        }

        private IEnumerable<IDictionary<string, object>> GetUsers()
        {
            var users = new List<IDictionary<string, object>>();
            foreach (var ldap in Ldaps)
            {
                var root = new DirectoryEntry(ldap, UserName, Password);
                DirectorySearcher searcher = new DirectorySearcher(root);
                searcher.Filter = UserFilter;
                searcher.PropertiesToLoad.AddRange(UserProperties.ToArray());
                searcher.PageSize = int.MaxValue;
                var results = searcher.FindAll();
                foreach (SearchResult result in results)
                {
                    var entry = result.GetDirectoryEntry();
                    Dictionary<string, object> data = new Dictionary<string, object>();

                    if (string.IsNullOrWhiteSpace(UserIdField))
                        data.Add("id", entry.Guid);
                    else
                        data.Add("id", entry.Properties[UserIdField].Value);

                    if (string.IsNullOrWhiteSpace(UserParentIdField))
                        data.Add("parentId", entry.Parent.Guid);
                    else
                        data.Add("parentId", entry.Parent.Properties[UserParentIdField].Value);

                    //bool? isEnabled = null;
                    //if (entry.Properties["userAccountControl"].Value != null)
                    //    isEnabled = (((int)entry.Properties["userAccountControl"].Value & 0x0002) == 0);

                    //data.Add("isEnabled", isEnabled);

                    foreach (var propertyName in UserProperties)
                    {
                        object propertyValue = null;
                        if (propertyName == "objectSid")
                        {
                            propertyValue = new SecurityIdentifier(entry.Properties["objectSid"].Value as byte[], 0).ToString();
                        }
                        else
                        {
                            if (result.Properties[propertyName].Count > 0)
                            {
                                propertyValue = result.Properties[propertyName][0];
                            }
                        }
                        data.Add(propertyName, propertyValue);
                    }
                    users.Add(data);
                }
            }
            return users;
        }

        private IEnumerable<IDictionary<string, object>> GetGroups()
        {
            var users = new List<IDictionary<string, object>>();
            foreach (var ldap in Ldaps)
            {
                var root = new DirectoryEntry(ldap, UserName, Password);
                DirectorySearcher searcher = new DirectorySearcher(root);
                searcher.Filter = GroupFilter;
                searcher.PropertiesToLoad.AddRange(GroupProperties.ToArray());
                searcher.PageSize = int.MaxValue;
                var results = searcher.FindAll();
                foreach (SearchResult result in results)
                {
                    var entry = result.GetDirectoryEntry();
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    if (string.IsNullOrWhiteSpace(GroupIdField))
                        data.Add("id", entry.Guid);
                    else
                        data.Add("id", entry.Properties[GroupIdField].Value);

                    if (string.IsNullOrWhiteSpace(GroupParentIdField))
                        data.Add("parentId", entry.Parent.Guid);
                    else
                        data.Add("parentId", entry.Parent.Properties[GroupParentIdField].Value);
                    //data.Add("parentId", entry.Parent.Guid);
                    //bool? isEnabled = null;
                    //if (entry.Properties["userAccountControl"].Value != null)
                    //    isEnabled = (((int)entry.Properties["userAccountControl"].Value & 0x0002) == 0);

                    //data.Add("isEnabled", isEnabled);

                    foreach (var propertyName in UserProperties)
                    {

                        object propertyValue = null;
                        if (propertyName == "objectSid")
                        {
                            propertyValue = new SecurityIdentifier(entry.Properties["objectSid"].Value as byte[], 0).ToString();
                        }
                        else
                        {
                            if (result.Properties[propertyName].Count > 0)
                            {
                                propertyValue = result.Properties[propertyName][0];
                            }
                        }

                        data.Add(propertyName, propertyValue);
                    }
                    users.Add(data);
                }
            }
            return users;
        }

        private IEnumerable<IDictionary<string, object>> GetDepartments()
        {
            var orgs = new List<IDictionary<string, object>>();
            foreach (var ldap in Ldaps)
            {
                var root = new DirectoryEntry(ldap, UserName, Password);
                DirectorySearcher searcher = new DirectorySearcher(root);
                searcher.Filter = OrganizationalUnitFilter;
                searcher.PropertiesToLoad.AddRange(OrganizationalUnitProperties.ToArray());
                searcher.PageSize = int.MaxValue;
                var results = searcher.FindAll();
                foreach (SearchResult result in results)
                {
                    var entry = result.GetDirectoryEntry();
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    if (string.IsNullOrWhiteSpace(OrganizationIdField))
                        data.Add("id", entry.Guid);
                    else
                        data.Add("id", entry.Properties[OrganizationIdField].Value);

                    if (string.IsNullOrWhiteSpace(OrganizationParentIdField))
                        data.Add("parentId", entry.Parent.Guid);
                    else
                        data.Add("parentId", entry.Parent.Properties[OrganizationParentIdField].Value);

                    foreach (var propertyName in OrganizationalUnitProperties)
                    {
                        object propertyValue = null;
                        if (result.Properties[propertyName].Count > 0)
                        {
                            propertyValue = result.Properties[propertyName][0];
                        }
                        data.Add(propertyName, propertyValue);
                    }
                    orgs.Add(data);
                }
            }
            return orgs;
        }
    }

    [Flags]
    public enum QuerySchema
    {
        user = 1,
        organizationalUnit = 2,
        group = 4
    }
}
