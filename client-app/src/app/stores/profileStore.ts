import { RootStore } from './rootStore'
import { action, observable, runInAction, computed, reaction } from 'mobx'
import agent from '../api/agent'
import { IProfile, IPhoto } from '../models/profile';
import { toast } from 'react-toastify';


export default class ProfileStore {
  rootStore: RootStore
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore
    reaction(
      () => this.activeTab,
      activeTab => {
        if (activeTab === 3 || activeTab === 4) { const predicate = activeTab === 3 ? 'followers' : 'followings'; this.loadFollowings(predicate) }
        else this.followings = []
      }
    )
  }

  @observable profile: IProfile | null = null
  @observable loadingProfile = true
  @observable uploadingPhoto = false
  @observable loading = false
  @observable followings: IProfile[] = []
  @observable activeTab: number = 0

  @computed
  get isCurrentUser() {
    if (this.rootStore.userStore.user && this.profile) { return this.rootStore.userStore.user.username === this.profile.username }
    else return false
  }

  @action setActiveTab = (activeIndex: number) => { this.activeTab = activeIndex }

  @action
  loadProfile = async (username: string) => {
    this.loadingProfile = true
    try {
      const profile = await agent.Profiles.get(username)
      runInAction(() => { this.profile = profile; this.loadingProfile = false })
    }
    catch (error) { console.log(error); runInAction(() => this.loadingProfile = false) }
  }

  @action
  uploadPhoto = async (file: Blob) => {
    const user = this.rootStore.userStore.user; this.uploadingPhoto = true
    try {
      const photo = await agent.Profiles.uploadPhoto(file)
      runInAction(() => {
        if (this.profile) {
          this.profile.photos.push(photo)
          if (photo.isMain && user) { user.image = photo.url; this.profile.image = photo.url }
          this.uploadingPhoto = false
        }
      })
    }
    catch (error) { console.log(error); toast.error('Problem uploading photo'); runInAction(() => this.uploadingPhoto = false) }
  }

  @action
  setMainPhoto = async (photo: IPhoto) => {
    const user = this.rootStore.userStore.user; this.loading = true
    try {
      await agent.Profiles.setMainPhoto(photo.id)
      runInAction(() => {
        if (user && this.profile) {
          user.image = photo.url; this.profile.image = photo.url
          this.profile.photos.find(a => a.isMain)!.isMain = false
          this.profile.photos.find(a => a.id === photo.id)!.isMain = true
        };
        this.loading = false
      })
    }
    catch (error) { console.log(error); toast.error('Problem setting main photo'); runInAction(() => this.loading = false) }
  }

  @action
  deletePhoto = async (photo: IPhoto) => {
    this.loading = true
    try {
      await agent.Profiles.deletePhoto(photo.id)
      runInAction(() => {
        if (this.profile) this.profile.photos = this.profile.photos.filter(x => x.id !== photo.id)
        this.loading = false
      })
    }
    catch (error) { console.log(error); toast.error('Problem deleting photo'); runInAction(() => this.loading = false) }
  }
  
  @action
  updateProfile = async (profile: Partial<IProfile>) => {
    const user = this.rootStore.userStore.user
    try {
      await agent.Profiles.updateProfile(profile)
      runInAction(() => {
        if (this.profile && user) {
          if (user.displayName !== profile.displayName) user.displayName = profile.displayName!
          this.profile = { ...this.profile, ...profile }
        }
      })
    }
    catch (error) { console.log(error); toast.error('Problem updating profile') }
  }

  @action
  follow = async (username: string) => {
    this.loading = true
    try {
      await agent.Profiles.follow(username)
      runInAction(() => { this.profile!.following = true; this.profile!.followersCount++; this.loading = false })
    }
    catch (error) { console.log(error); toast.error('Problem following user'); runInAction(() => this.loading = false) }
  }

  @action
  unfollow = async (username: string) => {
    this.loading = true
    try {
      await agent.Profiles.unfollow(username)
      runInAction(() => { this.profile!.following = false; this.profile!.followersCount--; this.loading = false })
    }
    catch (error) { console.log(error); toast.error('Problem unfollowing user'); runInAction(() => this.loading = false) }
  }

  @action
  loadFollowings = async (predicate: string) => {
    this.loading = true
    try {
      const profiles = await agent.Profiles.listFollowings(this.profile!.username, predicate)
      runInAction(() => { this.followings = profiles; this.loading = false })
    }
    catch (error) { console.log(error); toast.error('Problem loading followings'); runInAction(() => this.loading = false) }
  }
}
