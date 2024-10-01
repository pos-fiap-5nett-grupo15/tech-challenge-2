﻿using ContactsManagement.Application.DTOs.Contact.GetContatListPaginatedByFilters;

namespace ContactsManagement.Application.Interfaces.Contact.GetContatListPaginatedByFilters;

public interface IGetContactListPaginatedByFiltersHandler
    : IRequestHandler<GetContatListPaginatedByFiltersRequest, GetContatListPaginatedByFiltersResponse> { }
