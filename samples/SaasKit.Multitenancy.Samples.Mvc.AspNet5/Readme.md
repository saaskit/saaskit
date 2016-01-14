# Prerequisites

- Install DNVM and set to correct version (rc1-final as of time of writing, but check project.json)


#How to run via command line

- Open Command Prompt as Administrator
    - Change directory to root of SaasKit.Multitenancy.Samples.Mvc.AspNet5 project
    - run: dnx web
- Open web browser to tenant domain


#How to run via Visual Studio 2015 Debugger

- Within VS, right click on tool bar and ensure that the "Debug" toolbar is checked
- On the Debug toolbar ensure the buttons "Solution Configurations" and "Debug Target" are checked
- In the "Debug Target" button (probably has a green arrow with the label "IIS Express" showing initially)
    - click the drop down arrow
    - select "webTenant1" or "webTenant2"
- Run or debug as per usual (eg "Ctrl+F5" or "F5")
- Open web browser to tenant domain


# Tenant URLS

- Tenant 1: http://localhost:5000
- Tenant 2: http://localhost:5001
- unknown tenant: http://localhost:5009