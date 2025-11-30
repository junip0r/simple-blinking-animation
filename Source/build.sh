#!/bin/bash

set -e

case "$1" in
    debug|'')
        dotnet build --configuration Debug "-p:DefineConstants=\"$DEFINE\""
        cp -a bin/Debug/net472/BlinkingAnimation.{dll,pdb} ../Assemblies
        ;;
    release)
        dotnet build --configuration Release "-p:DefineConstants=\"$DEFINE\""
        cp -a bin/Release/net472/BlinkingAnimation.dll ../Assemblies
        rm -f ../Assemblies/BlinkingAnimation.pdb
        ;;
    *)
        echo "Usage: $(basename "$0") [debug|release]"
        false
        ;;
esac
