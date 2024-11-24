# MetaCraft

MetaCraft is a simple, limited-scope and to an extent, self-contained package
management software that works on binary packages.

The purposes of MetaCraft is to be used with other programs that produces and
gather binary packages that MetaCraft consumes.

## Usage

If you have .NET SDK 8.0 available and your shell is POSIX compatible, a shell
script called `run` is available that allows testing in a standalone package
scope.

```plain
Usage:
  MetaCraft [command] [options]

Options:
  -?, -h, --help  Show help and usage information
  --version       Show version information

Commands:
  info, inspect <file>         Inspects the metadata of a binary package
  install <files>              Installs a binary package into the current scope
  remove <packages> <version>  Removes a package from the current scope []
```

## Building

You'll need .NET SDK 8.0. Navigate into `src` with your favourite shell and run
`dotnet build`.

## Contributing

Pull requests are welcome. For major changes, please open an issue so proposed
stuff can be discussed first. Please don't surprise us with big pull requests.

Changes to specification requires proposal first. Please also update tests if
necessary.

## Licence

[GPL-3.0-or-later](COPYING)
