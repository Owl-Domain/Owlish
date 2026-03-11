# Owlish

<!-- Do not put the link/image nested tags on new lines as that will count the links as having whitespace which changes the rendering -->

<p align="center"> <!-- Organisation -->
  <a title="A link to the OwlDomain Discord server." href="https://discord.gg/JtXMeqVGQc"><img alt="Status badge for the OwlDomain discord server." src="https://img.shields.io/discord/1411024983550853162?style=social&logo=discord&label=discord&link=https%3A%2F%2Fdiscord.gg%2FJtXMeqVGQc"></a>
</p>

---

**Owlish** is a terminal shell meant specifically for interactive use, since I
wanted to make one, and because none of the shells that I know of had the
interesting features that I wanted.


## Features

This project is obviously still a work-in-progress, and probably will be for a
while, but right now the features that I am hoping to support will include:

### Configuration profiles

This feature will allow for having different prompts and rules for how the shell
behaves under different circumstances, or in different directories.

For example, this will let you have different profiles for each one of your
programming projects, or a base profile for all of your projects, now I'm sure
you can think of how this can be useful to *you* better than I can.


### Improved history control

Along with the [configuration profiles](#configuration-profiles), you'll be able to have better history
control, this means that each profile could have its own independent history,
and different rules for what will be saved in the history. For example making
sure simple commands like `whoami` or `ls` don't pollute the history.

### Places (bookmarks?)

One thing that I've always liked, is programs that had different integrations.
Now this feature will be available without the OS integration, but I still think
it's a neat idea. So... what is this feature?

Well, I personally like having my shell prompt show the full directory in which
I'm currently in, but I've also noticed that sometimes this gets very lengthy
and I end up with something like this:

```
/mnt/data/projects/nightowl/projects/owlish/repo/src/common/Common/bin/Debug/net10.0
```

Obviously that is long *(honestly, big yikes energy)*, so what if instead we had
a feature where we can use the [configuration profiles](#configuration-profiles)
to "bookmark" certain directories, and using them to both shorten the prompt
path, but also allow them to be used in the shell command as replacements.

This would mean that if I was in the `mnt/data/projects/nightowl` directory,
then `@owlish` would point to the
`mnt/data/projects/nightowl/projects/owlish` directory, and therefore shortening
the prompt to just show:

```
@owlish/repo/src/common/Common/bin/Debug/net10.0
```

Or potentially even:

```
@owlish/@debug/net10.0
```

Now this is obviously much shorter and easier to deal with, and how these
bookmarks are created will be up to you, but I'm hoping to allow:

- Direct bookmarks, i.e. you hardcode an absolute/relative directory where the
  bookmark will point to.
- Or you say that the bookmarks should be dynamically discovered, i.e. "every
  git repository in this directory".
- And finally, I'd like there to be some OS integration, since most file
  managers have some form of bookmark/favourite folders, it'd be cool if the
  shell could integrate with those.

*<sub>
  FYI if you're wondering, that path was to a different version of this project,
  because damn I sure seem to love rewriting stuff.
</sub>*

### Command previews

I've also noticed that quite often people *(read: me)* will type in very simple
commands just because they need a quick reminder of what it outputs, for example
`pwd`,`whoami`, `date` or whatever else there is.

So I thought it'd be cool to make it so that you can mark specific inputs as
being allowed to run automatically, so that when you type in `whoami` *(or when
the autocomplete suggests it),* you'll have a preview of its output before you
even press enter.

Now of course for simple commands this might not be that useful, but you could
also do it for things like `cat` so that if the output is short enough you'll
get a preview of the file, and since you don't have to actually execute the
command *(unless you want to)* then the output won't actually be visually kept
in the terminal buffer, meaning you can still easily see what you had there
before, without having to scroll up past the massive file you just printed.

### Other types of previews?

Now speaking about [command previews](#command-previews), why should this only
be limited to previewing commands? I mean each time I use a shell there's
definitely a lot of empty space *below* the prompt, so why not put it to
some use?

Quick fire ideas for how this could be used during live prompt input:
- Showing what values the used variables resolve to, or explain if they're
  missing.
- Showing which executables are actually going to be executed.
- Showing when a file you're trying to run isn't executable, and then providing
  a simple shortcut to make it executable.
- Showing which argument the value that're you passing in corresponds to.
- Validating known argument/flag values.
- Explaining what alias you're going to be executing.
- More detailed explanation of changes in the git repository instead of forcing
  it all into the prompt.
- Output of a custom command you tell it to run, for example a script to run
  when entering a directory, or a constant
  [`fastfetch`](https://github.com/fastfetch-cli/fastfetch) dashboard for those
  elite "I use arch btw" linux users. \
  <sup>I use arch btw - well technically manjaro but close enough.</sup>


## Development

This package is being developed on the `develop` branch. The *(default)* `main`
branch will only be updated either when it's necessary, or when there's a new
release.
