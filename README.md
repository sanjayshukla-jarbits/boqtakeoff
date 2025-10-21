Git branch strategy that includes the main branches (Master, Pre-Production, Development) and how to organize feature and sub-feature branches. This strategy is commonly known as Gitflow.

1. **Master Branch:**
   - The master branch is the production-ready branch. It should always contain stable and release-ready code. Direct commits to this branch should only be made for hotfixes or urgent production changes.

2. **Pre-Production Branch:**
   - This branch is for pre-production testing and validation before promoting code to the production environment. It should reflect a stable version of the application that is ready for final testing.

3. **Development Branch:**
   - The development branch is the main integration branch for ongoing development work. Developers work in their feature branches and merge their changes into this branch for integration.

4. **Feature Branches:**
   - Feature branches are created for each new feature or enhancement. These branches are created from the development branch. Naming convention can be like `feature/feature-name`.

5. **Sub-feature Branches:**
   - If needed, for complex features, sub-feature branches can be created under the main feature branches. Naming convention can be like `feature/feature-name/sub-feature`.

Here's a step-by-step workflow:

- Developers create feature branches from the development branch for their assigned tasks.
- Work is done in feature branches. Commits are made to these branches.
- Once a feature is completed and tested, the feature branch is merged back into the development branch.
- Pre-production testing is done using the pre-production branch. If any issues are found, they are fixed in the feature branches and merged back into development, then pre-production.
- When it's time for a release, a release branch can be created from the development branch for final testing and minor adjustments.
- After successful testing on the release branch, it's merged into both master (for production deployment) and development (to ensure future development includes the release changes).

Remember to regularly merge the changes from the development branch into your feature branches to keep them up to date with the latest code and resolve any merge conflicts early. Also, ensure to follow a pull request/code review process before merging branches to maintain code quality and consistency.
