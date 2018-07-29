# ElvUI Updater

Updates your ElvUI. Pulls source code from the [ElvUI repo on GitLab](https://git.tukui.org/elvui/elvui).

Stores ElvUI source code in `%APPDATA%` and copies it into `${WoW}\Interface\AddOns\ElvUI(_Config)?`. 
Source code can be deleted as part of normal system cleanup if desired (though at time of writing it's only around 45 MB).

Uses [Costura.Fody](https://github.com/Fody/Costura) to embed [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) DLLs 
and other dependencies for (some) portability.
