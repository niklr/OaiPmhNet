using System.Xml.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet
{
    public static class OaiErrors
    {
        public static XElement Error(string code, string message)
        {
            return new XElement("error", message,
                new XAttribute("code", code));
        }

        public static XElement BadArgument => Error("badArgument",
            "The request includes illegal arguments, is missing required arguments, " +
            "includes a repeated argument, or values for arguments have an illegal syntax.");

        // Not allowed
        public static XElement BadArgumentMetadataPrefixNotAllowed => Error("badArgument",
            "The request includes a 'metadataPrefix' argument that is not allowed for this verb.");
        public static XElement BadArgumentResumptionTokenNotAllowed => Error("badArgument",
            "The request includes a 'resumptionToken' argument that is not allowed for this verb.");
        public static XElement BadArgumentIdentifierNotAllowed => Error("badArgument",
            "The request includes a 'identifier' argument that is not allowed for this verb.");
        public static XElement BadArgumentFromNotAllowed => Error("badArgument",
            "The request includes a 'from' argument that is not allowed for this verb.");
        public static XElement BadArgumentUntilNotAllowed => Error("badArgument",
            "The request includes a 'until' argument that is not allowed for this verb");
        public static XElement BadArgumentSetNotAllowed => Error("badArgument",
            "The request includes a 'set' argument that is not allowed for this verb.");

        // Illegal syntax / granularity
        public static XElement BadFromArgument => Error("badArgument",
            "The request includes a 'from' argument that has an illegal syntax / granularity.");
        public static XElement BadUntilArgument => Error("badArgument",
            "The request includes a 'until' argument that has an illegal syntax / granularity.");

        // Verb
        public static XElement BadVerb => Error("badVerb",
            "Value of the verb argument is not a legal OAI-PMH verb, " +
            "the verb argument is missing, or the verb argument is repeated.");

        // Metadata
        public static XElement BadMetadataArgument => Error("badArgument",
            "The request does not include the required 'metadataPrefix' argument.");
        public static XElement CannotDisseminateFormat => Error("cannotDisseminateFormat",
            "The value of the 'metadataPrefix' argument is not supported by repository.");
        public static XElement NoMetadataFormats => Error("noMetadataFormats",
            "There are no metadata formats available for the specified item.");

        // ResumptionToken
        public static XElement BadArgumentExclusiveResumptionToken => Error("badArgument",
            "The request includes a 'resumptionToken' argument that must be the only argument (in addition to the verb argument).");
        public static XElement BadResumptionToken => Error("badResumptionToken",
            "The value of the 'resumptionToken' argument is invalid or expired.");

        // Set
        public static XElement NoSetHierarchy => Error("noSetHierarchy",
            "The repository does not support sets.");

        // Other
        public static XElement BadIdentifierArgument => Error("badArgument",
            "The request does not include the required 'identifier' argument.");
        public static XElement IdDoesNotExist => Error("idDoesNotExist",
            "The value of the identifier argument is unknown or illegal in this repository.");
        public static XElement NoRecordsMatch => Error("noRecordsMatch",
            "The combination of the values of the 'from', 'until', 'set' and 'metadataPrefix' arguments results in an empty list.");
        public static XElement BadFromUntilCombinationArgument => Error("badArgument",
            "The 'from' argument must be less than or equal to the 'until' argument.");

        public static bool ValidateArguments(ArgumentContainer arguments, OaiArgument allowedArguments)
        {
            return ValidateArguments(arguments, allowedArguments, out XElement errorElement);
        }

        public static bool ValidateArguments(ArgumentContainer arguments, OaiArgument allowedArguments, out XElement errorElement)
        {
            errorElement = null;

            if (!allowedArguments.HasFlag(OaiArgument.MetadataPrefix) &&
                !string.IsNullOrWhiteSpace(arguments.MetadataPrefix))
            {
                errorElement = BadArgumentMetadataPrefixNotAllowed;
                return false;
            }

            if (!allowedArguments.HasFlag(OaiArgument.ResumptionToken) &&
                !string.IsNullOrWhiteSpace(arguments.ResumptionToken))
            {
                errorElement = BadArgumentResumptionTokenNotAllowed;
                return false;
            }

            if (!allowedArguments.HasFlag(OaiArgument.Identifier) &&
                !string.IsNullOrWhiteSpace(arguments.Identifier))
            {
                errorElement = BadArgumentIdentifierNotAllowed;
                return false;
            }

            if (!allowedArguments.HasFlag(OaiArgument.From) &&
                !string.IsNullOrWhiteSpace(arguments.From))
            {
                errorElement = BadArgumentFromNotAllowed;
                return false;
            }

            if (!allowedArguments.HasFlag(OaiArgument.Until) &&
                !string.IsNullOrWhiteSpace(arguments.Until))
            {
                errorElement = BadArgumentUntilNotAllowed;
                return false;
            }

            if (!allowedArguments.HasFlag(OaiArgument.Set) &&
                !string.IsNullOrWhiteSpace(arguments.Set))
            {
                errorElement = BadArgumentSetNotAllowed;
                return false;
            }

            return true;
        }
    }
}
