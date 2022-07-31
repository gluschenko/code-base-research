//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.16.1.0 (NJsonSchema v10.7.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import * as dayjs from 'dayjs';


export enum TestEnum {
    A = 1,
    B = 2,
}

export interface SpaceItemViewModel {
    Uid: string;
    Name?: string | null;
    IsChosen: boolean;
    Description?: string | null;
}

export interface NavLinkViewModel {
    Text: string;
    Url: string;
}

export interface AppDataViewModel {
    SidebarNavigation?: NavLinkViewModel[] | null;
}

