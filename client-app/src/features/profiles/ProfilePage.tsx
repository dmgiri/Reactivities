import React, { useContext, useEffect } from 'react'
import { Grid } from 'semantic-ui-react'
import ProfileHeader from './ProfileHeader'
import ProfileContent from './ProfileContent'
import { RootStoreContext } from '../../app/stores/rootStore'
import { RouteComponentProps } from 'react-router'
import LoaderComponent from '../../app/layout/LoaderComponent'
import { observer } from 'mobx-react-lite'


interface RouteParams { username: string }
interface IProps extends RouteComponentProps<RouteParams> {}

const ProfilePage: React.FC<IProps> = ({ match }) => {

  const rootStore = useContext(RootStoreContext)
  const { profile, loadProfile, loadingProfile } = rootStore.profileStore

  useEffect(() => { loadProfile(match.params.username) }, [loadProfile, match])
  if (loadingProfile) return <LoaderComponent content='Loading profile...' />

  return (
    <Grid>
      <Grid.Column width={16}>
        <ProfileHeader profile={profile!} />
        <ProfileContent profile={profile!} />
      </Grid.Column>
    </Grid>
  )
}

export default observer(ProfilePage)
