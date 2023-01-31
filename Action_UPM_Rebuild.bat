git branch -D upm
git push origin -d upm
git subtree split --prefix=Assets/XenoIK --branch upm
git push origin upm --tags