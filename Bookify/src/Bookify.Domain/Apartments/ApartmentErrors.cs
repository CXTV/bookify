﻿using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class ApartmentErrors
{
    public static Error NotFound = new(
        "Apartment.NotFound",
        "The apartment with the specified identifier was not found");

    public static Error NotReadyError = new(
        "Apartment.NotReadyError",
        "The apartment with the specified identifier was not found");
}
