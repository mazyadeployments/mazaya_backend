#H2 Deployment

#H3 File Structure

devops
- Offers2
	- source
	- target

#H3 File Structure - Explaind

devops - folder for all devops related projects
Offers2 folder with sources from both source control-s
source - main/development source control
target - azure devops source control

#H3 Usage - copy SC-s

1. clone main/dev repo to source folder
2. clone devops repo to target folder
3. checkout dev or some other branches in both sc-s
4. go to source and run copyFromRepoToDevops script using PowerShell
5. commit and push in target folder

