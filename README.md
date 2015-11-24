# The Dargon Project
Dargon is NOT FEATURE COMPLETE nor FULLY FUNCTIONAL! See the [Issue Tracker](https://github.com/the-dargon-project/the-dargon-project/issues) for more information.

### Tl;Dr
The Dargon Client facilitates the safe installation of League of Legends modifications. We are unique in that we apply modifications in-memory; no game data is directly overwritten, which prevents client data corruption. We hope to support other games in the future.

Nest/Dargon (aka Dargopia) aims to become a .net application framework. End goals and key strengths should be client-side deployment, building multiprocess applications, and self-hosting at small-to-medium scale.

### Maintainers and Contributors
You may contact me at [/r/dargon](//reddit.com/r/dargon) or [@ItzWarty](//twitter.com/ItzWarty).

We want more developers! If you're looking to help out, there's a lot of up-for-grabs and cool stuff to be done.

Individuals who have contributed to Dargon in no particular order: Michael "ItzWarty" Yu, Adrian Astley, Ryan LaSarre, Eduardo Cabrera.

Special thanks goes to: Brian Chan, Richard Min, and a ton of mentors. Also, velkor2, yourbuddypal, themantrum, and GorbyRU for their efforts back in 2011.

## Building The Dargon Client
* git clone [this repository] .
* git submodule update --init
* explorer dargon-root.sln (or just manually open it)
* build the entire project to restore nuget packages
* git reset --hard
* build again.

## Terminology (It's confusing)
* the dargon project - the overall name of this effort.  
* dargopia - our joke way of referring to all this work.
* dargon client - refers to League of Legends modding application.
    * client - the GUI of the Dargon Client.
    * core - daemon which does most of the client's busy work.
    * trinket - hijacks other processes to override behavior.
* server - server-side code.
    * platform - backend code.
    * webend - e.g. APIs exposed over port 80.
    * hydar - experimental (wip) in-memory data grid.
* nest - application deployment, patching, bootstrapping.
* libraries
    * courier - udp-focused networking library
    * management - jconsole for .net - lets you create debugging endpoints for applications without creating a proper gui.
    * portable-objects - serialization library
        * pof-streams - exposes pof streams and provides utilities for consuming them.
    * ryu - IoC container integrated with Dargon's tech
    * services - remote-method-invocation library
    * random crap
        * audits - wip, owner Brian Chan
        * filesystems - abstracts filesystems, was once considered a good idea
        * io - exposes filesystems as nodes.
        * itzwarty.commons - useful extensions for .net
        * itzwarty.proxies - proxy classes useful for testability
        * scene - 3d scene-building api (wip)
        * systemstate - useful for feature-toggle functionality
        * league of legends - it's a fun game
            * rads - reads Riot Application Distribution System files
            * wgeo - reads map data
        * libvfm - creates virtual file descriptors to tell trinket how files should look.