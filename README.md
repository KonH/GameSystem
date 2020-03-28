[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# GameSystem

## Overview

Repository contains shared codebase for game projects to speed up prototyping & development process.

## Disclaimer

This project is created for self-development and fun, do not use solutions from it directly, without any skepticism.

Prefer to use cloud service or dedicated server, do not manage all hardware and software by yourself, it's hard and waste of your time if you're just a game developer. But it's interesting for better tech understanding.

Select your game architecture wisely, proposed one isn't suitable for all game types.

Performance is not goal of that project, many solutions can be much more faster, but it leads to detailed profiling and ugly code. It isn't required for that moment. 

In real world projects it's really hard to create universal codebase for projects and business needs is more important that developer understanding of quality and extendability.
And small side projects is really fine for prototyping such ideas. 

## Key features

- Generic command pattern
- Client-server REST communication
- Data persistence
- Unity async extensions
- Unity DI service provider

## Basics

Almost all classes based on generic parameters **TConfig** and **TState** implements IConfig and IState.
It's allow to create shared logic between different projects.

### Config

Config contains static information about game settings like balance, prices, event descriptions and so on.
It should contains ConfigVersion **Version** property to select proper version for given client.

### State

State is related to specific user and contains all game progress information (max level, scores, etc).
It should contains StateVersion **Version** property to detect state conflicts and reject unwanted destructive updates.

## Generic command pattern

Use command class like that to update game state:

```c#
[TrustedCommand] // Use it in case that command is allowed to invoke by client-side
class YourCommand : ICommand<GameConfig, GameState> {
	public CommandResult Apply(GameConfig config, GameState state) {
		// Validation
		if ( ... ) {
			return CommandResult.BadCommand(...);
		}
		// Update state
		return CommandResult.Ok();
		}
	}
```

If you need to run several commands in chain, add it using queue:
```c#
class CommandQueue : CommandQueue<GameConfig, GameState> {
	public CommandQueue() {
		AddDependency<FirstCommand, SecondCommand>(CreateSecondCommand);
	}

	SecondCommand CreateSecondCommand(GameConfig config, GameState state, FirstCommand cmd) {
		// Calculate second command parameters
		return new SecondCommand();
	}
}
```

If some commands in chain failed to execute, then initial command will be rejected on server-side.

Sometimes you need to add side effects for command execution (like animations), use reactions:
```c#
class YourCommandReaction : UnityCommandReaction<GameConfig, GameState, YourCommand> {
	public override async Task AfterOnMainThread(GameConfig config, GameState state, YourCommand command) {
		// Reaction logic 
	}
}
```


## Clicker project

### Overview

Increase your resources via click and spend it to boost single click resource income.

Play it [here](https://konhit.xyz/ClickerUnityClient/).

### Features

- Run in any modern web browser
- Persist game progress between sessions
- Basic cheat-protection

### Screenshots

Unity WebGL client:

![unityClient](Png/Clicker/unityClient.png)

Alternative console client:

![consoleClient](Png/Clicker/consoleClient.png)

### Tech

Basic architecture is simple:

![architecture](Png/Clicker/architecture.png)

Project uses many parts of shared code and its [common logic](Clicker.Common) relatively small.
Several types of databases can be used on server side to save game process between player sessions and server reboots.

And it's run on Raspberry Pi server:

![infrastructure](Png/Clicker/infrastructure.png)

**Nginx** is used as a reverse-proxy for web service and static web server for client content, also it handle SSL connections to support HTTPS for client-server communications.

**CouchDB** is used instead of MongoDB because of Mongo limitations for actual versions (now it's support only x64 architecture and it leads to some tricky workarounds). CouchDB, built from sources, works very well in that scenario.

**System.d** simplify service maintenance, logging and run-on-startup behaviour.  

CI/CD process is automated using [TeamCity](.teamcity/settings.kts) on dedicated Windows PC, connected to Raspberry Pi server:

![deploy](Png/Clicker/deploy.png)

Dedicated PC is used because of Unity dependency, it isn't support ARM to run Editor, which is required to make builds.

## Internal tips

### Prepare new project

1) Copy .gitignore from similar project, add additional entries
2) If it's a service, copy .service file too (and replace all unique fields)

### Start service via systemd manually

1) Copy %PROJECT_NAME%/%SERVICE_NAME%.service to /etc/systemd/system
2) `sudo systemctl daemon-reload`
3) `sudo systemctl enable %SERVICE_NAME%`
4) `sudo systemctl start %SERVICE_NAME%`
