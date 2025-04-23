
SCK Changelog

Asset Version: 2.10.0


WARNING: This update introduces assembly definitions, a restructured folder system, and updated namespaces. 

If you have already installed a previous version of SCK in your project, installing this update may require a significant amount of work to:

- Manually update namespaces used by your own scripts that refer to SCK classes.

- If you already added your own assembly definitions and restructured the SCK folder, assemblies will not be automatically updated, as updates do not modify script locations or add new references to your assemblies. You may have to fix lots of broken assembly definition references, or remove SCK completely and reinstall to use the assembly structure that comes with the kit instead.


Major additions/improvements
---------------------------------------------------------------------------------------------

- Added assembly definitions to speed up compilation time.

- Added gamepad controls menu showing currently bound controls for different vehicle classes.

- HUD Vehicle Parts Health: display health of subsystems on the HUD and run visual effects for damage/healing. Implemented in capital ship demo.

- Radar Filters: the Tracker now keeps a list of Radar Filter components to determine what can be tracked at any time. Radar Filter components can be extended to easily customize tracking behaviour.

- Scene Physics Manager: Added to control physics settings per-scene, e.g. disabling gravity in space scenes.

- Added Beam Visual Effects Controller and Beam Sound Effects Controller components for more modular customisation of beam weapons.

- Added components to manage playing sound effects when a vehicle is damaged, healed, or collides (Vehicle Damage Shot Audio, Vehicle Damage Looped Audio, Vehicle Collision Audio).

- Aim Controller: Removed a lot of functionality that doesn't belong in this class and put it in the classes it belongs (e.g. Weapons Controller and Target Selector). This component is now used by other components to get aim information, rather than doing work outside its scope.

- Added modular weapons models kit.

- Added auto-turrets for all weapon types that can be scaled to any size.

- Added player-controllable turrets for all weapon types.

- Added new demo scene: SCK_Loadout_CapitalShips - capital ship loadout.

- Added new demo scene: SCK_CarrierLaunch - demo of a space fighter launching from a carrier.

- Added new demo scene: AutoTurrets - demo of autoturrets for all gun types.

- Added new demo scene: EnterableTurrets - demo of enterable turrets for all gun types.



Minor additions/improvements
---------------------------------------------------------------------------------------------

Controls:

- Improved the mouse/keyboard controls menu


Vehicles:

- Module Mount: Removed 'Sorting Index'. Module mounts are now added to the vehicle in the order at which they appear in the hierarchy.

- Vehicle: Added option to restore the vehicle when it's enabled in the scene (if it was destroyed).

- IModulesUser: Interface added so that scripts can get notified about modules without having to extend ModuleManager base class.


UI:

- UI Fill Bar: Added stepping options so that the bar can fill in steps of a certain size.

- Button State: Added unity events for when button state becomes the priority state for the button, or becomes not the priority state.

- Added UIFillBar component as base class for all fill bar graphics.


Loadout:

- Loadout stats: stats now use Monobehaviours so you can easily modify/override/replace with your own.

- Loadout Manager: Added Unity Actions for when vehicle selection or module selection changes.

- Loadout: Now selects first available module on the UI whenever new module mount is selected.

- Loadout Display Manager: Added option to position vehicles in the loadout according to their bounds center point (not transform position). 

- Loadout Vehicle Item: Added a list of module mount viewing angles so you can customize the camera view angle for each module mount in the loadout.

- Loadout Camera Controller: Improved loadout camera behaviour.


Spaceships:

- Ship Lander: Added 'Can Launch' function to add conditions for whether a ship can launch.

- Capital ship: fixed hull colliders so turrets aren't embedded in them.

- Capital ships demo: added limited boost fuel.

- Capital Ship Controls: Added zoom controls with mouse scrollwheel.

- Capital ships demo: Added fire to destroyed subsystems.

- Engines: Added function to clear boost inputs (so you can e.g. clear boost via events when vehicle destroyed).


Vehicle Control Animations:

- Rigidbody Control Animations: Renamed from 'Vehicle Control Animator'.

- Rigidbody Control Animations: Fixed bug with lerp speed not being applied.

- Rigidbody Control Animations: Added setting to pitch based on vertical speed (useful for capital ships).

- Rigidbody Control Animations: Removed independent update, now must be used together with a Vehicle Control Animation Handler.

- Rigidbody Control Animations: Added customizable roll limit.

- Rigidbody Control Animations: Added roll animation relative to rigidbody roll speed.

- Steering Follow Animator: Removed (Rigidbody Control Animations already does this).


Radar:

- Trackable: Added Root Trackable getter.

- Trackable: Child trackables are no longer added in inspector, but collected automatically down the hierarchy.

- Trackable Module Linker: Now has option to override the Trackable Type and/or Label of module Trackable components.

- Trackable: Added functions to activate or deactivate child trackables (so that new child trackables added at runtime respect the parent activation state).

- Subsystem Target Box: Improved UI.


Weapons:

- Weapons Controller: Added functions to get the lead target position for the weapon with the highest or lowest speed.

- Beam Hit Effect Controller: Added a hit effect controller base class you can customize with your own code, and simplified the example controller.

- Projectile: Added option to scale hit effects.

- Weapons: Added functions to easily set the damage and healing multipliers for weapons at runtime.

- Turret Controller: Set the team here and it will update all relevant components on the turret with team information.

- Target Selector: Added Random target selection function.

- Weapon: Removed LinkedConditions and replaced with List<Condition>.


Cameras:

- Vehicle Camera: Now looks through entire vehicle hierarchy for a Camera Target, not just the root transform.

- Vehicle Death Camera Controller: Updated to follow a moving vehicle while it is detonating.


Object Pooling:

- Pool Manager: Objects are now created as children of an inactive gameobject so that their Awake and OnEnable functions are not called until they are requested from the pool.

- Pool Manager: Added option to return an object inactive. Enables customisation of settings before OnEnable is called on that object's scripts.

- Object Spawner: Now waits until Start method is called before the OnEnable code is triggered. Prevents unwanted spawning by pooled objects that are immediately disabled.


Effects:

- Added Object Scale Controller to manage scaling of objects with visual effects that don't automatically scale (e.g. line renderers, light range, particle system gravity).

- Energy Shield Controller: now accounts for scaling of the shield mesh when sending position information to the shader.

- Animated Renderer: Renamed to Renderer Color Controller, updated and improved.

- Visual Effects Controller: Renamed from Effects Color Controller, updated and improved.

- Effect Spawner: removed (Object Spawner base class now has all its functionalities).

- Engines Exhaust Controller: Added animated lights.

- Engine Audio Controller: Fixed bug with axis contributions not implemented correctly.

- Add Rumble: Made all the inspector properties gettable/settable.



Floating Origin:

- Floating Origin Object: Now finds necessary components even on gameobjects that are deactivated.

- Floating Origin Manager: Added list of conditions that must be met for the floating origin to update.


Misc:

- Removed unnecessary packages from asset store upload.

- Removed unnecessary TextMeshPro example assets.

- Removed unnecessary Package Manager Checker.

- Removed unnecessary Linked Conditions.

- Deprecated/removed the Energy System, has been replaced with Resources System.

- Renamed HUDDistanceLookup to DistanceStringLookup.

- Removed unused Behaviour Blackboard class.

- Removed unused Group Member class.

- Removed unused Game State Compatibility Checker.

- Removed unnecessary PlayerInput_InputSystem_GeneralControls script (duplicate of PlayerInput_InputSystem_MenuControls).



Bugfixes
---------------------------------------------------------------------------------------------

- Menu Controller: Fixed bug where it didn't collect menu state components that were disabled at the start.

- Vehicle Enter Exit Manager: Fixed bug where player could get in and out of a vehicle from anywhere if they exited outside the vehicle's enter/exit bubble.

- First Person Character Controller: Fixed bug where character exits vehicle with accumulated speed.

- Fixed bug where boost fuel kept going down after boost stopped.

- Animation Controller: Fixed bug where animation length was not being implemented correctly.

- Vehicle Engines 3D: Fixed bug where boost fuel was used when pressing boost even when boosting was disabled via modulation.

- Aim target selection: Fixed bug with aim target selection where the target closest to the reticle wasn't prioritized.

- Rigidbody Character Controller: Fixed bug where setting the velocity of the rigidbody when it's kinematic causes a warning.

- Triggerable: Fixed bug where Triggerable waited an extra Action Interval at the end of a burst.

- Projectile: fixed bug where area effect damage was called twice when missile collided with an object.

- Loadout Item Info Controller: Fixed bug where info was not updated when the component was enabled after loadout is active.

- Projectile: fixed bug with excessive damage with missiles. Collision scanner is now disabled upon detonation, and enabled again in OnEnable().

- Obstacle Avoidance Behaviour: Fixed bug with Obstacle Avoidance Margin not being implemented.
