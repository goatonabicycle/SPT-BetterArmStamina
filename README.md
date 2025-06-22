
# BetterArmStamina

In Escape From Tarkov you're meant to be a soldier, right? Not a software engineer. Surely you could rest your arm against your knee much longer when holding a gun? Surely when you lay down, you'll naturally rest your arms on the gun and the gun on the ground and be comfortable for much longer? Surely!

This is a mod for SPTarkov that provides more realistic arm stamina draining when aiming down sights (ADS) based on your stance and weapon mounting. It's entirely configurable so you can set the drain rate of various states to your liking.

## Features

You can adjust the ADS arm stamina drain when using bipods or when in standing, crouch, and prone stances.

- **Standing**: Sure, this should drain at some reasonable pace (100% drain rate by default)
- **Crouching**: Your PMC can hold his aim a bit longer when he's got a knee down. Pretend you're resting your elbow on your knee for stability (60% drain rate by default)
- **Prone**: When you're laying on the ground, you can hold your aim way longer (30% drain rate by default)
- **Mounted**: Press V near a mountable surface or deploy a bipod and you can hold your aim way longer (30% drain rate by default)

## Compatibility

This should be compatible with most mods except for ones that tinker with arm stamina drain rates. Will likely conflict with Realism one day.

## Installation

Just like any other client side plugin, extract the release archive into your `SPT/BepInEx/plugins/` folder.

## Configuration

You can configure the stamina drain rates in the BepInEx Configuration Manager (F12 in game) 

If you don't like the default values, just change them! They're percentages, so:
- 100 = Normal drain rate (vanilla)
- 50 = Half drain rate
- 200 = Double drain rate
