Setting up this repository on GitHub
=================================

Follow these steps after creating the repository on GitHub and pushing this code.

1. Push this repository to GitHub
   - Create an empty repo on GitHub (for example: `NetAdmin`).
   - Locally, add the remote and push:
     - `git remote add origin git@github.com:<your-org-or-username>/<repo>.git`
     - `git push -u origin main`

2. Enable GitHub Actions CI
   - The workflow file is `.github/workflows/ci.yml` and will run on pushes and PRs.
   - The workflow is named `build-and-test` and the job ID is `build`. The required status check will appear as the job name in branch protection settings (look for `build` under the workflow).

3. Configure branch protection for `main`
   - Go to the repository Settings > Branches > Add rule.
   - Branch name pattern: `main`.
   - Check `Require status checks to pass before merging` and select the `build` status (from the `build-and-test` workflow).
   - Check `Require pull request reviews before merging`.
   - Check `Require review from Code Owners` (this will force an approval from the CODEOWNERS entry, see next step).
   - (Optional) Uncheck `Include administrators` if you want the rules to apply to administrators as well.

4. Restrict who can push to `main`
   - In the same branch protection rule, enable `Restrict who can push to matching branches`.
   - Add the GitHub user `henrikmaurin` (your account) as the only allowed user. This prevents direct commits to `main` by others.

5. CODEOWNERS
   - The file `.github/CODEOWNERS` designates `@henrikmaurin` as code owner for all paths. With `Require review from Code Owners` enabled, PRs affecting any file will require your approval before merging.

6. Pull request process
   - Contributors may open PRs from their branches. The PR must pass CI and receive your review/approval before it can be merged.

7. Badge (optional)
   - After the first successful workflow run, add a workflow badge to the README. Example (replace owner/repo):
     - `![build-and-test](https://github.com/<owner>/<repo>/actions/workflows/ci.yml/badge.svg?branch=main)`

If you want, I can add a GitHub Actions workflow that also publishes artifacts or runs additional checks (linters, formatters). Do you want that? 
