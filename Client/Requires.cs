//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient
{
    public static class Requires
    {

        private const string ResourceIdValidator = "^[a-zA-Z0-9-]{36}$";

        /// <summary>
        /// Checks argument value
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="name">Name of argument</param>
        /// <param name="value">Value of argument</param>
        /// <returns>The <see cref="ArgumentRequirements"/> for this argument</returns>
        public static ArgumentRequirements<T> Argument<T>(string name, T value)
        {
            return new ArgumentRequirements<T>(name, value);
        }

        public struct ArgumentRequirements<T>
        {
            public string name;
            public T value;

            /// <summary>
            /// Initializes a new instance of the ArgumentRequirements struct
            /// </summary>
            /// <param name="name">The name</param>
            /// <param name="value">The value</param>
            public ArgumentRequirements(string name, T value)
            {
                this.name = name;
                this.value = value;
            }

            /// <summary>
            /// Checks argument value for not null
            /// </summary>
            /// <param name="arg1">The arg1 requirement</param>
            /// <param name="arg2">The arg2 requirement</param>
            public static void AnyNotNull(ArgumentRequirements<T> arg1, ArgumentRequirements<T> arg2)
            {
                if (arg1.value == null && arg2.value == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' and '{1}' cannot be null at the same time", arg1.name, arg2.name), arg1.name);
                }
            }

            /// <summary>
            /// Checks argument value not null or empty
            /// </summary>
            /// <param name="arg1">The arg1 requirement</param>
            /// <param name="arg2">The arg2 requirement</param>
            public static void AnyNotNullOrEmpty(ArgumentRequirements<T> arg1, ArgumentRequirements<T> arg2)
            {
                AnyNotNull(arg1, arg2);

                string stringValue1 = arg1.value as string;
                string stringValue2 = arg2.value as string;
                if (string.IsNullOrWhiteSpace(stringValue1) && string.IsNullOrWhiteSpace(stringValue2))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' and '{1}' cannot be empty at the same time", arg1.name, arg2.name), arg1.name);
                }
            }

            /// <summary>
            /// Checks whether argument is greater than zero.
            /// </summary>
            /// <returns>throws Validation Exception if value is not greater than zero</returns>
            public ArgumentRequirements<T> GreaterThanZero()
            {
                var comparable = this.value as IComparable;
                if (comparable == null || comparable.CompareTo(default(T)) <= 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} with value {1} is not larger than zero", this.name, this.value));
                }

                return this;
            }

            /// <summary>
            /// Checks whether argument matches a specified regex
            /// </summary>
            /// <returns>The valid regex requirement</returns>
            public ArgumentRequirements<T> MatchesRegex(string regex)
            {
                this.NotNullOrEmpty();

                string stringValue = this.value as string;
                if (!new Regex(regex).IsMatch(stringValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} is not valid", this.name));
                }

                return this;
            }

            /// <summary>
            /// Checks whether argument is not negative.
            /// </summary>
            /// <returns>throws a validation exception if the value is negative</returns>
            public ArgumentRequirements<T> NotNegative()
            {
                var comparable = this.value as IComparable;
                if (comparable == null || comparable.CompareTo(default(T)) < 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} with value {1} is negative", this.name, this.value));
                }

                return this;
            }

            /// <summary>
            /// Checks argument value for not null
            /// </summary>
            /// <returns>The not null requirement</returns>
            public ArgumentRequirements<T> NotNull()
            {
                if (this.value == null)
                {
                    throw new ArgumentNullException(this.name);
                }

                return this;
            }

            /// <summary>
            /// Checks argument value for not null or empty
            /// </summary>
            /// <returns>The not null or empty requirement</returns>
            public ArgumentRequirements<T> NotNullOrEmpty()
            {
                this.NotNull();

                string stringValue = this.value as string;
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot be empty", this.name), this.name);
                }

                return this;
            }

            /// <summary>
            /// Checks argument value for not including 4-byte unicode characters
            /// </summary>
            /// <returns>The no 4-byte unicode requirement</returns>
            public ArgumentRequirements<T> NoFourByteUnicode()
            {
                string stringValue = this.value as string;

                // A 4-byte unicode character occupies 4 bytes in both UTF32 and Unicode (UTF16) encoding 
                if (UTF32Encoding.UTF32.GetByteCount(stringValue) != UnicodeEncoding.Unicode.GetByteCount(stringValue) * 2)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot include invalid international characters", this.name), this.name);
                }

                return this;
            }

            /// <summary>
            /// Checks whether argument is a valid TCP port number
            /// </summary>
            /// <returns>The valid name requirement</returns>
            public ArgumentRequirements<T> ValidTCPPortNumber()
            {
                var portNumberValue = this.value as int?;
                if (portNumberValue != null && portNumberValue >= 1 && portNumberValue <= 65535)
                {
                    return this;
                }
                else
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} with value {1} is not a valid TCP port number", this.name, this.value));
                }
            }


            /// <summary>
            /// Checks Length of an argument
            /// </summary>
            /// <param name="minLength">Min allowed length</param>
            /// <param name="maxLength">Max allowed length</param>
            /// <returns>The valid length requirement</returns>
            public ArgumentRequirements<T> ValidLength(uint minLength, uint maxLength)
            {
                this.NotNullOrEmpty();

                string stringValue = this.value as string;
                string lengthRegex = string.Format(CultureInfo.InvariantCulture, "^.{{{0},{1}}}$", minLength, maxLength);

                if (!new Regex(lengthRegex).IsMatch(stringValue))
                {
                    throw new ArgumentException(string.Format("Argument {0} should be between {1} and {2} characters long", this.name, minLength, maxLength));
                }

                return this;
            }

            /// <summary>
            /// Checks max Length of an argument
            /// </summary>
            /// <param name="maxLength">Max allowed length</param>
            /// <returns>The valid length requirement</returns>
            public ArgumentRequirements<T> ValidMaxLength(uint maxLength)
            {
                string stringValue = this.value as string;

                if (!string.IsNullOrEmpty(stringValue) && stringValue.Length > maxLength)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} should be less than {1} characters long", this.name, maxLength));
                }

                return this;
            }

            /// <summary>
            /// Checks whether an argument is a valid resource id
            /// </summary>
            /// <returns>The valid resource id requirement</returns>
            public ArgumentRequirements<T> ValidResourceId()
            {
                this.NotNullOrEmpty();

                string stringValue = this.value as string;
                if (!new Regex(ResourceIdValidator).IsMatch(stringValue))
                {
                    throw new ArgumentException(string.Format("Invalid resource identidier '{0}'.", this.value));
                }

                return this;
            }

            /// <summary>
            /// Checks whether argument is a valid subscription
            /// </summary>
            /// <returns>The valid subscription requirement</returns>
            public ArgumentRequirements<T> ValidSubscriptionId()
            {
                try
                {
                    this.ValidResourceId();
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException(string.Format("Invalid subscription ID '{0}'.", this.value));
                }

                return this;
            }

            /// <summary>
            /// Validates that an ICollection has at least one element. 
            /// </summary>            
            public ArgumentRequirements<T> NonZeroElementCount()
            {
                var collectionArgument = this.value as ICollection;
                if (collectionArgument == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' is not a collection type", this.name), this.name);
                }

                if (collectionArgument.Count == 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' has no elements", this.name), this.name);
                }

                return this;
            }

            /// <summary>
            /// Checks whether argument is a valid collection of subscriptions
            /// </summary>
            /// <returns>The valid subscriptions requirement</returns>
            public ArgumentRequirements<T> ValidSubscriptionIds()
            {
                this.NonZeroElementCount();

                var stringValues = this.value as ICollection<string>;
                if (stringValues == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' is not a collection of strings", this.name), this.name);
                }

                if (stringValues.Any(stringValue => !new Regex(ResourceIdValidator).IsMatch(stringValue)))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} with value {1} is not a valid collection of subscriptionIds", this.name, this.value));
                }

                return this;
            }


            /// <summary>
            /// Checks whether argument is greater than zero if present.
            /// </summary>
            /// <returns>throws Validation Exception if value is not greater than zero</returns>
            public ArgumentRequirements<T> OptionalGreaterThanZero()
            {
                var comparable = this.value as IComparable;
                if (comparable != null && comparable.CompareTo(default(T)) <= 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} with value {1} is not larger than zero", this.name, this.value));
                }

                return this;
            }
        }

    }
}
