using System;
using UnityEngine;

namespace Compass
{
    /// <summary>
    /// Bit mask selecting which navigation slots a <see cref="SelectionNavigator"/> belongs to.
    /// Slot names are editor-only and configured in Project Settings > Compass.
    /// </summary>
    [Serializable]
    public struct NavigationMask : IEquatable<NavigationMask>
    {
        [SerializeField]
        private int value;

        /// <summary>
        /// The raw bit mask.
        /// </summary>
        public int Value
        {
            readonly get => value;
            set => this.value = value;
        }

        public NavigationMask(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Every slot (-1).
        /// </summary>
        public static readonly NavigationMask Everything = new(-1);

        /// <summary>
        /// No slots (0).
        /// </summary>
        public static readonly NavigationMask Nothing = new(0);

        public static implicit operator int(NavigationMask mask) => mask.value;

        public static implicit operator NavigationMask(int value) => new(value);

        public static bool operator ==(NavigationMask left, NavigationMask right) =>
            left.value == right.value;

        public static bool operator !=(NavigationMask left, NavigationMask right) =>
            left.value != right.value;

        public static NavigationMask operator &(NavigationMask left, NavigationMask right) =>
            new(left.value & right.value);

        public static NavigationMask operator |(NavigationMask left, NavigationMask right) =>
            new(left.value | right.value);

        public static NavigationMask operator ~(NavigationMask mask) => new(~mask.value);

        public override readonly bool Equals(object obj) =>
            obj is NavigationMask other && Equals(other);

        public readonly bool Equals(NavigationMask other) => value == other.value;

        public override readonly int GetHashCode() => value.GetHashCode();

        public override readonly string ToString() => value.ToString();
    }
}
