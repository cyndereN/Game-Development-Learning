## System background, objectives, and preparations

- How did you know what you needed to do, and how did you prepare to tackle the problem?
    - Requirement analysis, know what we need to do. MoSCoW List, OKR of it
    - Research online, essays, similar projects, including the techincal stacks needed, algorithms, what might be the hardest par, normally how long will it take
    - Assemble the team, moscow list, gantt chart 
    - Design process, a desgin document is received, e.g., a game design document is created, and the game's story, characters, and mechanics are developed. APIs
    - In some cases in software engineering project, TDD, write tests ahead of time

- Was there any missing information that you needed in order to start your work? How did you handle that?
    - Yes, for example the details of the project is there any technique that must be used. 
    - Normally I went to the clients or in some cases, teacher. In the Wapets2.0 I asked my client who is the target user? like what is this pet for? elderly people? teenagers? or anyone? because it would be different in models we choose, ways of interaction depending on which user is this product for. btw it is usually not the case that one product is designed for everyone. And have you conduct a survey to find out what is their actual pain point and need? Then he told me he haven't, we just need assume it is for elderly person. Also we must use IBM Watson for AI part
    - Sometimes they want us to make the decision. Like For example, waht engine to use to develop this game. We will have to make a table comparing each engine, e.g. name.. pro.. con.. and figure out which is the best fit. 

## System design

- Explain the overall architecture of the system and any interdependencies or interactions with other systems.

- What parts did you build or design, and what parts required collaboration? How did you collaborate?
    - Message(email) ahead, go to seat to ask in some cases, direct.
    - Use asana in my team (with pm and engineer), gitlab with my mentor, in-firm wiki communciation to seek for help, jira with QA and google document with designers.
    - Office hours 
    - weekly meeting, daily scrum
    - quick meeting on calender, if some urgent issue occurs
    - CI/CD, build a package, once it's done it automatically sends notification and download link to others who are relavant

- What were the key technical considerations, decisions, and tradeoffs?
    - Unity is bigger in a lot of ways — more market share, a larger asset library, and more devs to pick from. Unreal, on the other hand, is renowned for the unparalleled quality of its 3D assets and open-source nature. So, right out of the gate, there isn't an objectively better choice. We picked Unity because it has  I have previous knowledge for it which might leads to better result(grade).
    - Encapsulate watson, london traffic api in our backend or connect to them directly

    - Different ways to realise the system. Fall route. Find all the empty slots - this will lead to add components "empty cell" that has only one use case in ecs, it is such a waste - all other system are considering cell that elements this just does not fit in. Benifit: maybe we have more falling pattern in the future? the fall rout might be better. My mentor said it might be the case, that in the future our designers came up with something like this, but not for now, we have a ddl, to finish this ecs framework and there was more important part than this.
    - Dynamically add or delete element


## Delivery approach

- How did you validate that your work met the requirements?
    - TDD, pass all tests, 95% coverage.
    - MoSCoW Table. The requirements we have met are marked with "Finished" while those that we haven't are marked with an "X".
    - On asana, leader will approve if we finish; on gitlab, mentor will merge to main branch

- Did anything change during the course of development?
    - the amount really depends on how well we've done in design phase 
    - Abandon something that are not that necessary and takes much time than expected; usually features like "it is good to have", under "could-have" section of moscow list

- How was the system maintained over time, post-development?
    - check for updates of plugins regularly, compatibility issues
    - bug report from users, (us, most of the time)
    - development document in in-firm wiki, user manual, system manual
    - heavily commented 
    - design pattern used, like factory pattern, for readability and extendibility purposes when new functionality being added as a separate API


## Outcome and retrospective

- How did the project turn out?
    - very well. distinction. client speak highly of it
    - task finished shorter my mentor and leader said i am smarter and mroe hardworking than other interns
    - enter the semi-final of the contest, one of 15 or 20 teams

- What did you learn and what would you have done differently in hindsight?
    - knowledge, development process, technical, teamwork
    - TDD is really good. Write tests in advance, it might take a lot of time at the start, but overall the development process is shorter - debugging phase normally consumes more time.


## 处理服务器流量大？
- Load Balance
- Cache
- static html
- separate static and dynamic content, like images and js files
- mirror
- more server, distributed system