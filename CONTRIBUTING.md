![FaF Title Image](./FaF-GODOT/TitleBanner.png)

---

# Contributing to Fling And Fight
Looking to contribute to FaF? **Here's how you can help.**

Please take a moment to look over this document to make sure you follow all of the projects guidelines.

Going along with these contribution guidelines helps to look back on the projects history and saves time for the project maintainers.

**NOTE:** The /FaF-RBLX-ARCHIVE directory is not meant to be modified ever. It contains the old version of FaF built in Roblox for archival purposes. Knowing that, feel free to look at it and otherwise fork the game on Roblox. The RBXL file is now licensed under the same license as this project.

# Issues

The [github issue tracker](https://github.com/nikaxe-dev/fling-and-fight/issues) is the preferred channel for bug reports & feature requests, although they can also be put in the discord server.

Please follow these guidelines while making a issue:
* **Do not** use the issue tracker for help playing the game.
* **Do not** troll or otherwise distract conversations in issues. Keep on topic & use the discord server for any off topic conversations.
* **Do not** make issues for projects forked from Fling And Fight. This is for the official game only.
* **Do not** make issues about mods/addons for the game.
* **Do not** make duplicate issues without already checking for another issue of the same with good effort.

# Bug reports

A good bug report is one that describes the issue, gives what is expected from the user, and provides logs and/or evidence for the issue.

As such, we recommend you to include **these things** in your bug report:
* What is your issue?
* What do you expect when encountering this issue?
* How can you replicate this for yourself?
* Evidence as in screenshots/video of the bug in action alongside logs if captured.

# Feature Requests

Whether a feature is accepted or not is **completely up to the developers**. **Do not** have sour feelings over one of your feature requests being denied.

At the end of the day, it is our decision. And if you want to be sure that your idea fits within the game, check out our section on the goals of FaF in this document.

Please provide as much detail and context on how your feature will work.

# Pull Requests

Patches, codebase improvements, added documentation, & new features are all heavily valued by the developers. The more done, the quicker the game reaches completion.

Pull requests **must** fit within the goals of the project specified in this document.

**We cannot stress enough**: please check with the developers before starting any time consuming pull requests. We don't want anyone to waste their time working on the game just to get their changes rejected.

**IMPORTANT**: By submitting a pull request or patch, you agree to the [license](./LICENSE) on your code & assets alongside the fact that github will permanently store your given details publicly.

## Commits

Commits in this repository follow a certain structure to make the creation of changelogs easier to write & the contents of a commit easier to see from just the summary.

It is unlikely for your PR to get implemented if you do not follow this.

The format used for commit summaries is as follows:

`[optional_feature]:[optional_second_feature]/[type]:[optional_second_type]/[progress]: [summary]`

Only put the progress if it isn't complete.

#### Commit Types:
* fix
* chore
* docs
* feat
* refactor
* rewrite
* tiny-edit

#### Progress types:
* progress
* preparations

# Code documentation

Currently, the code documentation is quite limited in many areas and could be improved on quite a bit. Any additions to it are appreciated.

# AI guidelines

FaF is a passion project, meant to be created by people.

**Do not** create issues or pull requests that have been generated exclusively or heavily through the usage of an LLM or any other AI tools.
**Anybody who does this will risk being removed from the project with their issue or PR closed.**

When submitting a PR, you are required to be able to read & understand every single line of code modified, removed, or added from the project. Failure to do so will cause the PR to be closed.

I do not support the use of AI in creating a good chunk of a project.

# Project Goals

## Keeping to the premise of the original game built in RBLX.

Fling And Fight is a multiplayer physics sandbox game. It will stay so.

- Gears, items (rebranded to props), & other content features will stay the same.
- Other features such as the quick spawn menu or the view model will stay the same.
- The grabline physics will be replicated as closely as possible to the original game.
- Many textures from the original will be brought over to the new project.

However, not everything can stay the same.
## What is different?

- Systems will be improved.
- Multiplayer will be purposed for small groups of friends or singleplayer sessions. This is no longer Roblox, big game servers will not work.
- Branding will be different - better logo - added title logo, an actual trailer.
- Worlds - the game is split into worlds. Currently only New Sedes is planned, but in the future new worlds will be able to be easily added.
- Events - the original game had a problem. There wasn't many ways to get money. In this new version, badges will award content / money & events which award the same will be everywhere spread along the worlds.
- Avatars - built in the game, no longer external. Maybe accessories could grant special abilities? Still thinking.

## I wanna take the game in a different direction, can I?

Yes!! One of the great things about an open source game is the modding ability! The game already is able to be modded by adding worlds, but for a more modified version of the game you can fork the project and do whatever you wish with the game.
