//-----------------------------------------------------------------------------------
// <copyright file="EnsureUnique.cs" company="Sitecore Shared Source">
// Copyright (c) Sitecore.  All rights reserved.
// </copyright>
// <summary>
// Defines the Sitecore.Sharedsource.Rules.Actions.EnsureUnique type.
// </summary>
// <license>
// http://sdn.sitecore.net/Resources/Shared%20Source/Shared%20Source%20License.aspx
// </license>
// <url>http://marketplace.sitecore.net/en/Modules/Item_Naming_rules.aspx</url>
//-----------------------------------------------------------------------------------

using Sitecore.Data.Items;
using Sitecore.Rules;
using System;

namespace Sitecore.Sharedsource.ItemNamingRules.Actions
{
    /// <summary>
    ///     Rules engine action to ensure unique item names under a parent
    ///     by replacing trailing characters with a date/time stamp.
    /// </summary>
    /// <typeparam name="T">Type providing rule context.</typeparam>
    public class EnsureUnique<T> : RenamingAction<T> where T : RuleContext
    {
        /// <summary>
        ///     Gets or sets the maximum allowed length for item names.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        ///     Action implementation.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        public override void Apply(T ruleContext)
        {
            if (ruleContext.Item.Name.Length > MaxLength)
            {
                RenameItem(
                    ruleContext.Item,
                    ruleContext.Item.Name.Substring(0, MaxLength));
            }

            bool unique;

            do
            {
                unique = true;

                foreach (Item child in ruleContext.Item.Parent.Children)
                {
                    if (child.ID.Equals(ruleContext.Item.ID)
                        || !child.Key.Equals(ruleContext.Item.Key))
                    {
                        continue;
                    }

                    unique = false;
                    string strDateTime = DateUtil.ToIsoDate(
                        DateTime.Now).ToLower();
                    string newName = ruleContext.Item.Name + strDateTime;

                    if (MaxLength > 0 && newName.Length > MaxLength)
                    {
                        newName = newName.Substring(
                            0,
                            MaxLength - (strDateTime.Length + 1)) + strDateTime;
                    }

                    RenameItem(ruleContext.Item, newName);
                    break;
                }
            } while (!unique);
        }
    }
}