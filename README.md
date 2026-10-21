# ic-experiment-center-integration

Provides schemas and .NET code for interacting with iC Experiment Center.

## Introduction

iC Experiment Center allows users to carry out experiments in the laboratory.  It integrates with instruments and Mettler Toledo instrument software and has a public API to enable customer integrations with other systems like Electronic Lab Notebooks. This project has the schemas for using the public API of experiment center and .NET clients for accessing the public API of iC Experiment Center. It also has an adapter project that mimics [iC Data Centers](https://www.mt.com/us/en/home/products/L1_AutochemProducts/automated-reactor-in-situ-analysis-software/ic-data-center.html) file based integration pattern with iC Experiment Center. This adapter can be used by users of iC Experiment Center to replicate the iC Data Center workflow and also provides an example of using the iC Experiment Center public API.

## Documentation and examples

Links to docs, usage examples, and sample code.

## Installation / Setup

For the initial development phase the packages are available as artifacts of develop builds.

## Development

How to build, run, and test locally.

To build the solution directly, run `dotnet build src\MettlerToledo.Labs.Experiments.slnx`

## Contributing

How to submit a pull request. All commits require [DCO sign-off](link-to-dco-explanation) — no CLA required. See [CODE_OF_CONDUCT.md](https://github.com/MettlerToledo/.github/blob/main/CODE_OF_CONDUCT.md) before opening an issue or PR.

## Security

To report a security vulnerability, do not open a public issue — see [SECURITY.md](https://github.com/MettlerToledo/.github/blob/main/SECURITY.md) for the disclosure process and contact.

## License

Licensed under [MIT] — see [LICENSE](LICENSE) for details.
