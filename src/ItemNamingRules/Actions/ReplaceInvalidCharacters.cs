//-----------------------------------------------------------------------------------
// <copyright file="ReplaceInvalidCharacters.cs" company="Sitecore Shared Source">
// Copyright (c) Sitecore.  All rights reserved.
// </copyright>
// <summary>
// Defines the 
// Sitecore.Sharedsource.Rules.Actions.ReplaceInvalidCharacters 
// type.
// </summary>
// <license>
// http://sdn.sitecore.net/Resources/Shared%20Source/Shared%20Source%20License.aspx
// </license>
// <url>http://marketplace.sitecore.net/en/Modules/Item_Naming_rules.aspx</url>
//-----------------------------------------------------------------------------------

using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sitecore.Sharedsource.ItemNamingRules.Actions
{
    /// <summary>
    ///     Rules engine action to replace invalid characters in item names.
    /// </summary>
    /// <typeparam name="T">Type providing rule context.</typeparam>
    public class ReplaceInvalidCharacters<T> : RenamingAction<T>
        where T : RuleContext
    {
        /// <summary>
        ///     Gets or sets the string with which to replace invalid characters
        ///     in item names.
        /// </summary>
        public string ReplaceWith { get; set; }

        /// <summary>
        ///     Gets or sets the regular expression used to validate each character
        ///     in item names.
        /// </summary>
        public string MatchPattern { get; set; }

        /// <summary>
        ///     Action implementation.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        public override void Apply(T ruleContext)
        {
            Assert.IsNotNull(ReplaceWith, "ReplaceWith");
            var patternMatcher = new Regex(MatchPattern);
            string newName = String.Empty;

            foreach (char c in ruleContext.Item.Name)
            {
                if (patternMatcher.IsMatch(c.ToString(CultureInfo.InvariantCulture)))
                {
                    newName += c;
                }
                else if (!String.IsNullOrEmpty(ReplaceWith))
                {
                    newName += ReplaceWith;
                }
            }

            while (newName.StartsWith(ReplaceWith))
            {
                newName = newName.Substring(
                    ReplaceWith.Length,
                    newName.Length - ReplaceWith.Length);
            }

            while (newName.EndsWith(ReplaceWith))
            {
                newName = newName.Substring(
                    0,
                    newName.Length - ReplaceWith.Length);
            }

            string sequence = ReplaceWith + ReplaceWith;

            while (newName.Contains(sequence))
            {
                newName = newName.Replace(sequence, ReplaceWith);
            }

            if (String.IsNullOrEmpty(newName))
            {
                newName = ReplaceWith;
            }

            if (ruleContext.Item.Name != newName)
            {
                RenameItem(ruleContext.Item, newName);
            }
        }
    }
}