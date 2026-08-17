using System;
using System.Collections.Generic;
using System.Linq;
using BitwardenVaultManager.Service.Helpers;

namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenItem
    {
        static string UsernameFieldName => "Username";

        static string[] EmailAddressFieldNames => ["Email Address", "Email", "email", "input-login", "login_email", "sign_in_email"];

        static string[] PhoneNumberFieldNames => ["Phone Number", "phone", "phoneNumber", "phone_number", "billing_phone", "CellPhone", "form_creatang_telefon"];

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
                if (Fields is not null)
                {
                    foreach (string fieldName in EmailAddressFieldNames)
                    {
                        BitwardenField matchedField = Fields.FirstOrDefault(f => f.Name.Equals(fieldName));

                        if (matchedField is not null &&
                            !string.IsNullOrWhiteSpace(matchedField.Value) &&
                            matchedField.Value.Contains('@'))
                        {
                            return matchedField.Value;
                        }
                    }
                }

                if (Login is null)
                {
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(Login.Username) &&
                    Login.Username.Contains('@'))
                {
                    return Login.Username;
                }

                return null;
            }
        }

        public string PhoneNumber
        {
            get
            {
                if (Fields is not null)
                {
                    foreach (string fieldName in PhoneNumberFieldNames)
                    {
                        BitwardenField matchedField = Fields.FirstOrDefault(f => f.Name.Equals(fieldName));

                        if (PhoneNumberHelper.IsValid(matchedField?.Value))
                        {
                            return PhoneNumberHelper.Normalise(matchedField.Value);
                        }
                    }
                }

                if (Login is null)
                {
                    return null;
                }

                if (PhoneNumberHelper.IsValid(Login.Username))
                {
                    return PhoneNumberHelper.Normalise(Login.Username);
                }

                return null;
            }
        }

        public string Username
        {
            get
            {
                if (Fields is not null &&
                    Fields.Any(itemField => UsernameFieldName.Equals(itemField.Name)))
                {
                    return Fields.First(itemField => UsernameFieldName.Equals(itemField.Name)).Value;
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
