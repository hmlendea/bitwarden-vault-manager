using System;
using System.Collections.Generic;
using System.Linq;

namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenItem
    {
        static string EmailAddressFieldName => "Email Address";
        static string UsernameFieldName => "Username";

        public Guid Id { get; set; }

        public string Name { get; set; }

        public BitwardenItemType Type { get; set; }

        public Guid FolderId { get; set; }

        public bool IsFavourite { get; set; }

        public string Notes { get; set; }

        public BitwardenLogin Login { get; set; }

        public IEnumerable<BitwardenField> Fields { get; set; }

        public string EmailAddress
        {
            get
            {
                if (Fields is not null &&
                    Fields.Any(field => EmailAddressFieldName.Equals(field.Name)))
                {
                    return Fields.FirstOrDefault(field => field.Name == EmailAddressFieldName).Value;
                }

                if (Login is not null &&
                    !string.IsNullOrWhiteSpace(Login.Username) &&
                    Login.Username.Contains('@'))
                {
                    return Login.Username;
                }

                return null;
            }
        }

        public string Username
        {
            get
            {
                if (Fields is not null &&
                    Fields.Any(field => UsernameFieldName.Equals(field.Name)))
                {
                    return Fields.First(field => UsernameFieldName.Equals(field.Name)).Value;
                }

                if (Login is not null &&
                    !string.IsNullOrWhiteSpace(Login.Username))
                {
                    return Login.Username;
                }

                return null;
            }
        }
    }
}
