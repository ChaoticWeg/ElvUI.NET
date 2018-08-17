---
layout: home
redirect_from:
  - /elvui.net
---

<style type="text/css">
    .jumbotron {
        margin-bottom: 2em;
        padding-bottom: 3em;
        border-bottom: 1px solid #EEEEEE;
    }
    .jumbotron h1 {
        font-size: 5em;
    }
    .jumbotron a {
        margin: 0.5em;
    }

    h2 {
        margin-top: 1.5em;
    }
</style>

<div align="center" class="jumbotron">
    <h1>ElvUI.NET</h1>
    <a href="https://github.com/ChaoticWeg/elvui.net/blob/master/LICENSE"><img src="https://img.shields.io/github/license/ChaoticWeg/elvui.net.svg?style=for-the-badge"/></a>
    <a href="https://github.com/ChaoticWeg/elvui.net/releases/latest"><img src="https://img.shields.io/github/release/chaoticweg/elvui.net/all.svg?style=for-the-badge&label=Download"/></a>
    <br/><br/>
    <a href="https://ci.appveyor.com/project/ChaoticWeg/elvui-update"><img src="https://img.shields.io/appveyor/ci/chaoticweg/elvui-update.svg?logo=appveyor&style=for-the-badge"/></a>
    <a href="https://github.com/ChaoticWeg/elvui.net/issues"><img src="https://img.shields.io/github/issues/ChaoticWeg/elvui.net.svg?style=for-the-badge"/></a>
</div>

## How to Download

1. Click the Download button above
2. Download the .exe file under the most recent release

## How to Use

1. Enter your WoW install directory (or use the Select button on the right)
2. Click Update

## Important Notes

If Windows Defender SmartScreen is enabled, it will likely flag the executable as potentially dangerous the first time you run it. This is because it

- is not signed by a developer recognized by Microsoft
- is not signed with a certificate given by a trusted CA
- was downloaded from the internet

This may seem off-putting, and rightfully so &ndash; I am working on a fix for this (see issue [#3][issue-3]). Rest assured that SmartScreen is just doing its job to flag untrusted binaries from the internet.

In the meantime, if you would like, you can download a [freshly-built copy][artifact] from Appveyor.

[issue-3]: https://github.com/ChaoticWeg/elvui.net/issues/3
[artifact]: https://ci.appveyor.com/project/ChaoticWeg/elvui-update/build/artifacts
