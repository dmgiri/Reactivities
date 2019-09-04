export interface IProfile {
  displayName: string, 
  username: string,
  bio: string, 
  image: string,
  photos: IPhoto[]
}

export interface IProfileFormValues {
  displayName: string,
  bio?: string
}

export interface IPhoto {
  id: string,
  url: string, 
  isMain: boolean
}