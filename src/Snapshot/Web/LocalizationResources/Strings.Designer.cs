﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web.LocalizationResources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Web.LocalizationResources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is an automatically generated email message. Please do not reply..
        /// </summary>
        internal static string AutoGeneratedEmail {
            get {
                return ResourceManager.GetString("AutoGeneratedEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following sellers have not sent stock or sell updates in {0} days:
        ///.
        /// </summary>
        internal static string InactiveSellerDetectedBody {
            get {
                return ResourceManager.GetString("InactiveSellerDetectedBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inactive seller detected.
        /// </summary>
        internal static string InactiveSellerDetectedSubject {
            get {
                return ResourceManager.GetString("InactiveSellerDetectedSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect SMS alert.
        /// </summary>
        internal static string IncorrectSMSAlertSubject {
            get {
                return ResourceManager.GetString("IncorrectSMSAlertSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file selected is an invalid. Please choose another one..
        /// </summary>
        internal static string InvalidFileSelectedForUpload {
            get {
                return ResourceManager.GetString("InvalidFileSelectedForUpload", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Phone number not active. Please activate your phone number to send update stock messages..
        /// </summary>
        internal static string PhoneNumberNotActive {
            get {
                return ResourceManager.GetString("PhoneNumberNotActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seller: {1} {0}Product below limit: {2} {0}Current stock: {3} {0}Lower limit set at: {6} {0}Contact details: {4} {0}{0}{5}.
        /// </summary>
        internal static string StockBellowLimitEmailBody {
            get {
                return ResourceManager.GetString("StockBellowLimitEmailBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stock below limit.
        /// </summary>
        internal static string StockBellowLimitEmailSubject {
            get {
                return ResourceManager.GetString("StockBellowLimitEmailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seller {0} with phone number {1} has send the second consecutive invalid SMS. Please assist..
        /// </summary>
        internal static string TwoConsecutiveInvalidSMSEmailBody {
            get {
                return ResourceManager.GetString("TwoConsecutiveInvalidSMSEmailBody", resourceCulture);
            }
        }
    }
}
