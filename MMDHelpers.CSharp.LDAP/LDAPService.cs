using System;
using System.DirectoryServices;

namespace MMDHelpers.CSharp.LDAP
{
    public class LDAPService
    {
        private string path;

        public LDAPService(string Path)
        {
            this.path = Path;
        }

        public bool login(string user, string password)
        {

            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(path, user, password);
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = "(SAMAccountName=" + user + ")";
                SearchResult searchResult = directorySearcher.FindOne();
                if ((Int32)searchResult.Properties["userAccountControl"][0] == 512)
                {
                    return true;
                }
                else
                {
                    return false;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
