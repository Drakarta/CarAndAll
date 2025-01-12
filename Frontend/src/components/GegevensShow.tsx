export default function GegevensShow(props: any) {
    return (
      <div>
        <p>Naam: {props.user.name}</p>
        <p>Email: {props.user.email}</p>
        <p>Role: {props.user.role}</p>
        <p>Address: {props.user.address}</p>
        <p>Phone number: {props.user.phoneNumber}</p>
      </div>
    )
  }