# Prerequisites

- Install DNVM and set to correct version (rc1-final as of time of writing, but check project.json)


#How to run via command line

- Open Command Prompt as Administrator
	- Change directory to root of SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap project
	- run: dnx web
- Open web browser to tenant domain


# How to run via Visual Studio 2015 Debugger

- Within VS, right click on tool bar and ensure that the "Debug" toolbar is checked
- On the Debug toolbar ensure the buttons "Solution Configurations" and "Debug Target" are checked
- In the "Debug Target" button (probably has a green arrow with the label "IIS Express" showing initially)
	- click the drop down arrow
	- select "webTenant1" or "webTenant2 or "webTenant3"
- Run or debug as per usual (eg "Ctrl+F5" or "F5")
- Open web browser to tenant domain


# Tenant URLS

- Tenant 1: http://localhost:6001
- Tenant 2: http://localhost:6002
	- displays different message on the home page to other tenants due to change in StructureMap configuration
- Tenant 3: http://localhost:6003
- unknown tenant: http://localhost:6009


# Multitenancy with StructureMap

Please check "_Readme.md" in SaasKit.Multitenancy.AspNet5.StructureMap project