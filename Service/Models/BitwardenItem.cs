using System;
using System.Collections.Generic;
using System.Linq;

namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenItem
    {
        static string EmailAddressFieldName => "Email Address";

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
                if (!(Fields is null) &&
                    Fields.Any(field => field.Name.Equals(EmailAddressFieldName)))
                {
                    return Fields.FirstOrDefault(field => field.Name == EmailAddressFieldName).Value;
                }

                if (!(Login is null) &&
                    !string.IsNullOrWhiteSpace(Login.Username) &&
                    Login.Username.Contains('@'))
                {
                    return Login.Username;
                }

                return null;
            }
        }
    }
}
