import { RootStore } from './rootStore'
import { action, observable, runInAction, computed } from 'mobx'
import agent from '../api/agent'
import { IProfile } from '../models/profile'


export default class ProfileStore 
{
  rootStore: RootStore
  constructor(rootStore: RootStore) { this.rootStore = rootStore }

  @observable profile: IProfile | null = null
  @observable loadingProfile = true

  @computed
  get isCurrentUser() {
    if (this.rootStore.userStore.user && this.profile) { return this.rootStore.userStore.user.username === this.profile.username }
    else return false
  }

  @action
  loadProfile = async (username: string) => {
    this.loadingProfile = true
    try {
      const profile = await agent.Profiles.get(username)
      runInAction(() => { this.profile = profile; this.loadingProfile = false })
    }
    catch (error) { console.log(error); runInAction(() => this.loadingProfile = false) }
  }
}