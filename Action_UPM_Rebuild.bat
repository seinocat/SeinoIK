git branch -D upm
git push origin -d upm
git subtree split --prefix=Unity/Assets/XenoIK --branch upm
git push origin upm --tags