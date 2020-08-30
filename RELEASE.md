### Main repo
- upgrade version in Edgar.csproj
- merge dev to master
- git tag v2.0.0-alpha.2
- git push origin v2.0.0-alpha.2
- wait for build to finish
- release should be created as draft, add changelog

### Docs

- npm run version 2.0.0-alpha.1
- cmd /C "set "GIT_USER=OndrejNepozitek" && npm run deploy"
