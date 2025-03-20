The project is created in JetBrains Rider. 
The only external sources used for help is the MSDN Sources and ChatGPT o1 and Copilot from GitHub.

I have selected to use Blazor server rendering for the project because of the following reasons.
- To have a single codebase for the server and client.
- To ensure the complexity of the client dependencies to the backend is minimal.
    - I do not want to let the client have any domain critical logic and therefore the backend should handle it.
    - The responsibility of the client is to render the data and handle the user interaction and UI Components. ✨
- The problem giving in the assignments is to simply show data and store 1 user interaction. 

The solution uses the true and tested 3 layer architecture.
I have a crosscutting layer called Domain which contains the domain models and the interfaces for the services.

This is not to be confused with the Domain Driven Design, but the domain classes takes inspiration and have the responsibility to do calculations and hold the data.
I the case of this project, the domain classes are simple and only holds the data.

I have added Sqlite because it gives a fast way of having a database that can be run by others.
#Source: https://system.data.sqlite.org/src/doc/trunk/www/index.wiki

Timelog: 

19/03/2025 
30min - Creating boilerplate for the project.
20/03/2025
15min - Creating repo and uploading to github.

60min - Adding the Crud and SQLite to the project.
30min - Adding the web api, and printing the location data from sqlites to the blazor home page.
15min - Setting up basic navigation from home to the location page.
60min - Adding the rest of the data structure to SqLite and added the basic html with the data in it.
60min - Added more interactivity to the components so they share events and more when changing the state.