namespace MMA.WebApi.Shared.Constants
{
    public class UserTypeSequenceConstats
    {
        public static readonly int SequenceLengthWithoutPrefix = 12;
        public static readonly int SequenceLength = 7;

        public static readonly string ADNOCEmployee = "1971";
        public static readonly string ADNOCEmployeeSequencer = "dbo.ADNOCEmployeeSequencer";

        // This is changed because both Adnoc Employee and Adnoc Employee Familiy Member use same starting sequence
        // And eCard number needs to be unique, so they will both use same sequencer
        //public static readonly string ADNOCEmployeeFamilyMemberSequencer = "dbo.ADNOCEmployeeFamilyMemberSequencer";
        public static readonly string ADNOCEmployeeFamilyMember = "1971";
        public static readonly string ADNOCEmployeeFamilyMemberSequencer = "dbo.ADNOCEmployeeSequencer";

        public static readonly string ADPolice = "1957";
        public static readonly string ADPoliceSequencer = "dbo.ADPoliceSequencer";

        public static readonly string RedCrescent = "1983";
        public static readonly string RedCrescentSequencer = "dbo.RedCrescentSequencer";

        public static readonly string AlumniRetirementMember = "2018";
        public static readonly string AlumniRetirementMembersSequencer = "dbo.AlumniRetirementMembersSequencer";

        public static readonly string Etihad = "2003";
        // This sequencer doesn't exists yet
        //public static readonly string EtihadSequencer = "dbo.EtihadSequencer";

        public static readonly string ADPension = "2000";
        // This sequencer doesn't exists yet
        //public static readonly string ADPensionSequencer = "dbo.ADPensionSequencer";

        public static readonly string ALMasaoud = "1993";
        // This sequencer doesn't exists yet
        //public static readonly string ALMasaoudSequencer = "dbo.ALMasaoudSequencer";


        // This Type doesn't exists yet
        //public static readonly string ADSchools = "";
        public static readonly string ADSchoolsSequencer = "dbo.ADSchoolsSequencer";
    }
}
