# Security and public-source handling

Never post tokens, private signing keys, passwords, saved player profiles or raw
packet captures in public issues. Use a private reporting channel agreed with
the maintainer for sensitive findings; public reports should contain sanitized
reproduction steps only.

`publisher.json` is the upstream public verification key and download address,
not a credential for signing or pushing. PetTracker Aion V1 builds do not embed
it. Build tooling rejects that upstream publisher configuration. Fork release
and renewal workflows are gated to the upstream repository until a separate
fork publishing identity is deliberately configured.

GitHub workflow references to `github.token` and `secrets.*` are placeholders
resolved by GitHub at execution time. Their existence in YAML does not publish
the underlying secret. Actual repository settings, collaborators, Actions
secrets, deploy keys and GitHub push protection require a separate account-level
review; a source checkout cannot certify those settings.

Default V1 data: `%LOCALAPPDATA%\PetTrackerAion`.
Default V1 installation: `%LOCALAPPDATA%\Programs\PetTrackerAion`.
The upstream PetTracker data and installation directories are separate.
Explicit `--data` arguments still override the data location, including legacy
test launchers. Do not share a live data directory between running copies.

Source and history should be scanned before publication, not only ignored files.
Builds, player data, captures, key files and Visual Studio settings must remain
untracked. A scanner report can contain secrets: keep raw reports outside the
public source tree and redact all diagnostic output.

Detailed audit reports are kept privately and are excluded from source archives
and version control. Neither source inspection nor a clean dependency database
scan guarantees the absence of vulnerabilities or approval by the game publisher.
