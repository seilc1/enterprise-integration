
## Commit

Commits must follow the [Conventional Commit Guideline](https://www.conventionalcommits.org/en/v1.0.0/) as it is used
for creating the [semantic versioned](https://semver.org/) releases by using [GitVersion](https://gitversion.net/).

### Supported Types

fix, feat, chore, ci, docs, refactor, perf, test

### Major Increments

* if a commit contains either `BREAKING CHANGE` as part of the commit message.
* if the type is `feat`, `fix` or `perf` and post-fixed with an `!` (e.g: `feat!:`)

### Minor Increments

* if the type is `feat`

### Patch Increments

* if the type is `fix`, `chore`, `refactor`, `perf`, `docs`

### No Increments

* if the type is `build`, `ci`, `style`, `test`