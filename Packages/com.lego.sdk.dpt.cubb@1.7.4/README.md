#  Core Unity Bluetooth Bridge - CUBB

## Introduction

CUBB, Core Unity Bluetooth Bridge, is a LPAF component for integrating bluetooth communication with LEGO hubs/device into unity projects.

## Installation

Access to the CUBB package is via the Unity Package Manager system (UPM). To access the LEGO internal scoped registery containing the CUBB package it is necessary to add the scoped registery to your Unity apps' manifest.json file.

    {
        "scopedRegistries": 
        [
            {
            "name": "LEGO Private",
            "url": "https://upm.services.lego.com/private",
            "scopes":
            [
                "com.lego.sdk.dpt"
            ]
            }
        ],
        "dependencies": 
        {
        ....
        }
    }

An access token to the LEGO UPM registry is also required in the form of a .toml file. These can be obtained from DPT.

To incorporate CUBB into your Unity project from the Editor go to Window->PackageManager and select the LEGO registry. From there select the CUBB package and press the "install" button at the bottom right of the LEGO CUBB package description window. 

## Usage

## Dependencies & Structure

CUBB has dependencies to the follow submodule:
* Logger (com.lego.sdk.dpt.logger)


## Example
Sample scripts arre WIP

## Tests
There are no Unity unit tests included with this project. Unit tests reside in the native Swift or Java layer of the CUBB Native plugin that is included in this project.

## Build Instructions
Local builds - Download the CUBB Test App from https://gitlab.dto.lego.com/ and open the project found under the folder unity/cubb-unity in the Unity Editor, and select from the top dropdown menu the Build option, to further specify what platform and release type to build.

Build deployment - Tagging branches on the CUBB repo residing on https://gitlab.dto.lego.com/ will start build pipelines in Gitlab. The pipeline created for the tag contains manual stages for deploying build to App Center and to deploy UPM package of the CUBB to our scoped registry.


## Support

CUBB is developed and supported by LEGO Digital Product Technology (DPT) which is a part of LEGO System A/S  

## Status

Deployed and is a dependancy in all LPAF layers and applications that use the LPAF stack

## Copywrite and History

LEGO System A/S - created for LEGO originally by Trifork A/S
