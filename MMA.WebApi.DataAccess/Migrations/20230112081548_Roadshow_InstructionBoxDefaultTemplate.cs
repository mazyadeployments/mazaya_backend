using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Roadshow_InstructionBoxDefaultTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into [AdnocTermsAndConditions] (Content, ContentArabic, Type, CreatedBy, CreatedOn, UpdatedBy, 
                                    UpdatedOn)
                                    values ('<h2><strong>Firstly, find the below instruction for Al Ruwais Mall.</h2></strong>

<p>Mall Activation protocols:</p>
<or type=""1"">
<li>
All staff and contractors attending the kiosk should have a 48-hour valid PCR test result prior to beginning the first day at the mall. The test results should be emailed to Mr Galal (on copy) in advance and shown to security upon entry.
</li>
<li>
Contractors and staff must use the rear entrance of the mall and inform security upon first and last entry.
</li>
<li>
Any installation work at the kiosk to be carried out between 11pm and 9am.
</li>
<li>
All staff and contractors must wear facemasks at all times, maintaining 2m distance when possible while attending the kiosk. Previous staff attending the kiosk were found with masks off during operations which is not acceptable.
</li>
<li>
Hand-sanitizers must be available at the kiosk and used regularly by attending staff.
</li>
<li>
If there is any installation or electrical work, the Mall Work Permit form will be sent by Miriam (on copy) to you and is to be filled in prior to work commencing.
</li>
<li>
The commercial trade license copy should be available to view
</li>
<li>
If there are vouchers or giveaways, copy of DED promotional license should be available to view.
</li>
<li>
Staff must restrict sales activity and conversations with mall visitors to a 2-metre radius of the Mazaya kiosk. Roaming the mall in search of customers is not permitted. 
</li>
</or>
<h2><strong>Secondly find the below for all suppliers.</h2></strong>

<p>Please find the attached Roadshow proposal to be filled from your end.</p>
<or type=""1"">
<li>
ensure that they comply with the ADNOC Dress Code standards (see attached).Moreover, please provide us your materials list so that we can assist you with the security access and smooth entry. Also share with us your company flyers/collaterals mentioning the offers that will be presenting similar to attached. Our screen size is (W-1080px H- 1920px).
</li>
<li>
A negative result of COVID 19 test with 48 hours validity is mandatory along with the proof of vaccination (completed all doses).
</li>
<li>
2 presenters maximum should be in Mazaya Offers Booth. Social distance, wearing mask and gloves (if there is any kind of distribution).
</li>
<li>
Comply with the ADNOC Dress Code standards
</li>
<li>
All suppliers have to bring a flash memory regarding there announcement to be shared in the digital screen. 
</li>
</or>
', '', 5, '0C7B881A-F0C8-4B55-861B-A1E600AA6E11', '2023-01-09 13:08:40.9410605', '0C7B881A-F0C8-4B55-861B-A1E600AA6E11', '2023-01-09 13:08:40.9412141')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
