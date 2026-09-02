// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Enumeration for the classes of equipment understood in the system.
/// </summary>
public enum EquipmentCategoryJson
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Balance.
    /// </summary>
    Balance = 1,

    /// <summary>
    /// RTA IR analyzer device.
    /// </summary>
    IR = 2,

    /// <summary>
    /// An automated reactor system.
    /// </summary>
    Alr = 3,

    /// <summary>
    /// Equipment that transfers material and measures the amount of the material transferred in volume.
    /// </summary>
    VolumetricDosing = 4,

    /// <summary>
    /// Equipment that takes samples from a vessel. 
    /// </summary>
    Sampler = 5,

    /// <summary>
    /// Equipment that transfers material and measures the amount of the material transferred in mass.
    /// </summary>
    GravimetricDosing = 6,

    /// <summary>
    /// Thermometer for measuring temperature
    /// </summary>
    Thermometer = 7,

    /// <summary>
    /// A heater used for calorimetric calibration.
    /// </summary>
    CalibrationHeater = 9,

    /// <summary>
    /// A device that be controlled to specific value over a generic interface.
    /// That is we can control it but we don't really understand what the device does.
    /// </summary>
    FinalControlElement = 10,

    /// <summary>
    /// Used to pump liquid.
    /// </summary>
    LiquidPump = 11,

    /// <summary>
    /// A device that measures a process variable.
    /// </summary>
    Sensor = 12
}
